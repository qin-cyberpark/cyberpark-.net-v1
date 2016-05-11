namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.Serialization;
    using CyberPark.Domain.Utilities;
    public partial class Account
    {

        /// <summary>
        /// whether ready for issue invoice
        /// </summary>
        /// <returns></returns>
        public bool InvoiceIssuable(DateTime issueDate, ref string msg)
        {
            //whether existing
            if (IsLastInvoice(issueDate.Year, issueDate.Month))
            {
                msg = string.Format("invoice {0}/{1} is existing", issueDate.Year, issueDate.Month);
                return false;
            }

            //whether continuous
            var lstInv = LastInvoice;
            if (lstInv != null)
            {
                var lstInvMth = new DateTime(lstInv.Year, lstInv.Month, 1);
                var nxtInvMth = lstInvMth.AddMonths(1);
                if (nxtInvMth.Year != issueDate.Year || nxtInvMth.Month != issueDate.Month)
                {
                    msg = string.Format("can not issuse discontinuous invoice {0}/{1}, last={2}/{3}"
                                        , issueDate.Year, issueDate.Month, lstInv.Year, lstInv.Month);
                    return false;
                }
            }

            //check is external bill prepared
            //var dtFrom = new DateTime(year, month, 1);
            //var dtTo = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            if (!ExternalBill.HasExternalBillReadyForInvoice(issueDate))
            {
                msg = string.Format("external bills not ready, issue date {0}", issueDate);
                return false;
            }

            return true;
        }

        public IList<AddonCharge> GetAddonCharges(xISPContext db)
        {
            var charges = new List<AddonCharge>();
            db.Entry(this).Collection(x => x.AddonCharges).Query()
                                                  .Where(x => x.InvoiceId == null && !x.Ignored).Load();
            foreach (var ao in AddonCharges)
            {
                var converter = ExternalAddOnConverter.Match(ao.Description);
                if (converter != null)
                {
                    //matched and display
                    ao.DisplayDescription = string.IsNullOrEmpty(converter.DisplayAs) ? ao.Description : converter.DisplayAs;
                    ao.Charge = converter.Price ?? ao.Cost;
                    charges.Add(ao);
                }
                else
                {
                    //not matched
                    ao.Ignored = true;
                    ao.IgnoredBy = SysConfig.Instance.AutoOperatorId;
                }
            }

            db.SaveChanges();

            return charges;
        }

        public Invoice FillInvoice(xISPContext db, Invoice invoice, ref string msg)
        {
            //make product charge
            db.Entry(this).Collection(x => x.Products).Query().Where(x => !x.IsOneOff || (!x.HasOneOffCharged ?? false)).Load();


            foreach (var p in Products)
            {
                db.Entry(p).Collection(x => x.Services).Load();
                if (Service.Statuses.InService.Equals(p.Status))
                {
                    p.Charge(db);
                }
            }

            //add product charge
            db.Entry(this).Collection(x => x.ProductCharges).Query()
                                    .Where(x => x.InvoiceId == null).Load();

            foreach (var sc in ProductCharges)
            {
                invoice.Add(sc);
            }

            //make calling charge
            if (!CallingCharge.CalcCallingCharge(db, Id, ref msg))
            {
                return null;
            }

            //add call charge
            db.Entry(this).Collection(x => x.CallingCharges).Query()
                                    .Where(x => x.InvoiceId == null).Load();
            foreach (var cc in CallingCharges)
            {
                invoice.Add(cc);
            }
            db.Accounts.Attach(this);

            //add transaction
            db.Entry(this).Collection(x => x.Transactions).Query()
                                    .Where(x => x.InvoiceId == null && !x.IsDeleted).Load();
            foreach (var t in Transactions)
            {
                invoice.Add(t);
            }

            //add adjustment
            db.Entry(this).Collection(x => x.Adjustments).Query()
                                    .Where(x => x.InvoiceId == null && !x.IsDeleted).Load();
            foreach (var adj in Adjustments)
            {
                invoice.Add(adj);
            }

            //add addon charges
            var addonCharges = GetAddonCharges(db);
            foreach (var ao in addonCharges)
            {
                invoice.Add(ao);
            }

            return invoice;
        }

        /// <summary>
        /// issue invoice
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="staffId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Invoice IssueInvoce(xISPContext db, DateTime issueDate, int staffId, ref string msg)
        {
            using (var dbTrans = db.Database.BeginTransaction())
            {
                try
                {
                    //can issue or not
                    if (!InvoiceIssuable(issueDate, ref msg))
                    {
                        return null;
                    }

                    //issue
                    Invoice inv = new Invoice
                    {
                        AccountId = Id,
                        Year = issueDate.Year,
                        Month = issueDate.Month,
                        DateFrom = new DateTime(issueDate.Year, issueDate.Month, 1),
                        DateTo = new DateTime(issueDate.Year, issueDate.Month,
                                        DateTime.DaysInMonth(issueDate.Year, issueDate.Month), 23, 59, 59),
                        PreviousBalance = LastInvoice?.CurrentBalance ?? 0,
                        IssuedBy = staffId,
                        IssuedDate = DateTime.Now,
                        DisplayIssuedDate = NextInvoiceIssueDate?.Date ?? DateTime.Now
                    };

                    //invoice date for service given day
                    if (InvoicePeriodTypes.ServiceGivenDay.Equals(InvoicePeriodType))
                    {
                        inv.DateFrom = issueDate.AddMonths(-1);
                        inv.DateTo = issueDate.AddDays(-1);
                    }

                    FillInvoice(db, inv, ref msg);

                    //set invoice status
                    //auto check could be add here
                    inv.Status = Invoice.Statuses.Valid;
                    inv.AutoDeliver = true;

                    //seal all existing valid invoice
                    db.Invoices.Where(x => x.AccountId == Id && x.Status.Equals(Invoice.Statuses.Valid))
                               .ToList().ForEach(x => x.Status = Invoice.Statuses.Sealed);
                    db.SaveChanges();

                    //insert invoice
                    inv.Create(db);

                    //update balance
                    Balance -= inv.ChargeAmountIncludeGST;

                    //set next invoice issue date
                    NextInvoiceIssueDate = (NextInvoiceIssueDate?.Date ?? DateTime.Now).AddMonths(1);
                    db.Entry(this).State = EntityState.Modified;
                    db.SaveChanges();
                    dbTrans.Commit();

                    //safe pdf
                    inv.ToPDF();
                    return inv;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return null;
                }
            }
        }

        /// <summary>
        /// re-issue
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="staffId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Invoice ReissueInvoice(xISPContext db, int invoiceId, int staffId, ref string msg)
        {
            using (var dbTrans = db.Database.BeginTransaction())
            {
                //load
                var inv = Invoice.GetById(db, invoiceId, false);
                if (inv == null)
                {
                    msg = string.Format("Invoice {0} is not existing", invoiceId);
                    return null;
                }

                inv.CallingAmount = 0;
                inv.AddonAmount = 0;
                inv.TransactionAmount = 0;
                inv.AdjustAmount = 0;
                inv.ProductAmount = 0;
                inv.ChargeAmountExcludeGST = 0;
                inv.PreviousBalance = LastInvoice?.CurrentBalance ?? 0;
                inv.IssuedBy = staffId;
                inv.IssuedDate = DateTime.Now;

                //issue
                FillInvoice(db, inv, ref msg);
                inv.Status = Invoice.Statuses.Valid;

                //save
                db.Entry(inv).State = EntityState.Modified;

                //update balance
                Balance += inv.ChargeAmountIncludeGST;
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();

                dbTrans.Commit();
                return inv;
            }

        }

        public static void IssueAllInvoice(DateTime issueDate, int staffId)
        {
            Logger.Info("IssueAllInvoice", "start issue all invoice");
            //check is external bill prepared
            //external bill year/month
            var billYearMonth = new DateTime(issueDate.Year, issueDate.Month, 1).AddMonths(-1);
            if (DateTime.Today.Day <= SysConfig.Instance.ExternalBillImportDay)
            {
                //before external bill imported
                billYearMonth = billYearMonth.AddMonths(-1);
            }

            if (!ExternalBill.HasExternalBillReadyForInvoice(issueDate))
            {
                Warning.WriteAsync(WarningModules.Invoice, "IssueInvoiceAll",
                            string.Format("external bills not ready, issue date {0}", issueDate));

                Logger.Info("IssueAllInvoice", "end issue all invoice");

                return;
            }

            //load all account
            IList<Account> accounts;
            using (var db = new xISPContext())
            {
                accounts = db.Accounts.Include(x => x.Invoices).Where(x => x.IsActive
                        && x.NextInvoiceIssueDate <= issueDate).ToList();
                //loop
                string msg = null;
                foreach (var acct in accounts)
                {
                    if (acct.IsLastInvoice(issueDate.Year, issueDate.Month))
                    {
                        //has issued
                        continue;
                    }

                    if (acct.IssueInvoce(db, issueDate, staffId, ref msg) == null)
                    {
                        Warning.WriteAsync(WarningModules.Invoice, "IssueInvoiceAll",
                            string.Format("fail to issue invoice, {0}", msg), accountId: acct.Id);
                    }
                }
            }

            Logger.Info("IssueAllInvoice", "end issue all invoice");
        }

    }
}