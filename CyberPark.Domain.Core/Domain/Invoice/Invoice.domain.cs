namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using CyberPark.Domain.Utilities;

    public partial class Invoice
    {
        /// <summary>
        /// add transaction
        /// </summary>
        /// <param name="tr"></param>
        public void Add(Transaction tr)
        {
            if (tr != null)
            {
                Transactions.Add(tr);
                TransactionAmount += tr.Amount;
            }
        }

        public void Add(AddonCharge addOnRec)
        {
            if (addOnRec != null)
            {
                AddonCharges.Add(addOnRec);
                AddonAmount += addOnRec.Charge;
                ChargeAmountExcludeGST += addOnRec.Charge;
            }
        }

        public void Add(ProductCharge pdctRec)
        {
            if (pdctRec != null)
            {
                ProductCharges.Add(pdctRec);
                ProductAmount += pdctRec.AmountGSTExclusive;
                ChargeAmountExcludeGST += pdctRec.AmountGSTExclusive;
            }
        }

        public void Add(CallingCharge callChrg)
        {
            if (callChrg != null)
            {
                CallingCharges.Add(callChrg);
                CallingAmount += callChrg.Charge;
                ChargeAmountExcludeGST += callChrg.Charge;
            }
        }

        public void Add(Adjustment adj)
        {
            if (adj != null)
            {
                Adjustments.Add(adj);
                AdjustAmount += adj.Amount;
            }
        }

        [NotMapped]
        public double ChargeAmountIncludeGST
        {
            get
            {
                GST = ChargeAmountExcludeGST * 0.15D;
                return Math.Round(ChargeAmountExcludeGST + GST, 2);
            }
        }

        [NotMapped]
        public double CurrentBalance
        {
            get
            {
                return PreviousBalance - ChargeAmountIncludeGST + AdjustAmount + TransactionAmount;
            }
        }

        [NotMapped]
        public bool IsVoid
        {
            get
            {
                return Statuses.Void.Equals(Status);
            }
        }

        public static IList<Invoice> GetRecentInvoicesByAccountId(xISPContext db, int accountId, int rows = 10)
        {
            return db.Invoices.Where(x => x.AccountId == accountId).OrderByDescending(x => x.IssuedDate).Take(rows).ToList();
        }


        public static Invoice GetById(xISPContext db, int invoicdId, bool loadDetail = true)
        {
            Invoice invoice = null;
            if (loadDetail)
            {
                invoice = db.Invoices.Include(x => x.Account.Customer)
                                .Include(x => x.ProductCharges)
                                .Include(x => x.ProductCharges.Select(c => c.Product.Services))
                                .Include(x => x.Transactions).Include(x => x.Adjustments)
                                .Include(x => x.AddonCharges)
                                .Include(x => x.CallingCharges)
                                .Include(x => x.CallingCharges.Select(c => c.Service))
                                .Include(x => x.CallingCharges.Select(c => c.CallingRecords))
                                .SingleOrDefault(x => x.Id == invoicdId);
            }
            else
            {
                invoice = db.Invoices.SingleOrDefault(x => x.Id == invoicdId);
            }
            return invoice;
        }


        public static int GetUndeliveredInvoiceCount(xISPContext db)
        {
            return db.Invoices.Where(x => !Statuses.Void.Equals(x.Status) && x.DeliveredDate == null).Count();
        }

        public static bool Void(xISPContext db, int invoiceId, int userId)
        {
            using (var dbTrans = db.Database.BeginTransaction())
            {
                try
                {
                    var inv = GetById(db, invoiceId);
                    if (inv == null)
                    {
                        throw new Exception(string.Format("Invoice {0} is not existing", invoiceId));
                    }

                    if (inv.Status.Equals(Statuses.Sealed) || inv.Status.Equals(Statuses.Void))
                    {
                        throw new Exception(string.Format("Invoice {0} is Sealed or Void", invoiceId));
                    }

                    //set status
                    inv.Status = Statuses.Void;

                    //create history invoice
                    var voidInv = new VoidInvoice(inv, userId);
                    db.VoidInvoices.Add(voidInv);

                    //rollback product charge to date
                    foreach (var prdtChrg in inv.ProductCharges)
                    {
                        db.Entry(prdtChrg).Reference(x => x.Product).Load();
                        prdtChrg.Product.ChargedToDate = prdtChrg.PreviousProductChargedToDate;
                    }

                    //delete product charge
                    db.ProductCharges.RemoveRange(inv.ProductCharges);

                    //delete calling charge
                    db.CallingCharges.RemoveRange(inv.CallingCharges);

                    //relase addon charge
                    inv.AddonCharges.Clear();
                    //release transaction
                    inv.Transactions.Clear();
                    //relase adjustment
                    inv.Adjustments.Clear();

                    //rollback balance
                    inv.Account.Balance -= inv.ChargeAmountIncludeGST;

                    db.SaveChanges();
                    dbTrans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Invoice {0} is Sealed or Void", invoiceId), ex);
                }
            }
        }

        public void Create(xISPContext db)
        {
            db.Invoices.Add(this);
            db.SaveChanges();
        }

        public void Save(xISPContext db)
        {
            db.Invoices.Attach(this);
            db.SaveChanges();
        }

        public bool Delete(xISPContext db)
        {
            db.Invoices.Remove(this);
            db.SaveChanges();
            return db.Invoices.SingleOrDefault(x => x.Id.Equals(Id)) == null;
        }

        #region deliver
        public bool Deliver(xISPContext db)
        {
            string msg = null;
            try
            {
                if (MailHelper.Send(
                    "cupidshen@163.com", //Account.Email,
                    string.Format("CyberPark Statement {0}", Id),
                    Utility.ToJson(this), 
                    new string[] { PdfPath},
                    ref msg))
                {
                    DeliveredDate = DateTime.Now;
                    db.Entry(this).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    Logger.Error("DeliverInvoice", string.Format("failed to deliver invoice {0},{1} ", Id, msg));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DeliverInvoice", string.Format("failed to deliver invoice {0},{1} ", Id, ex.Message));
                return false;
            }
        }

        public static void DeliverAll()
        {
            Logger.Info("DeliverAllInvoice", "start deliver all invoice");
            //load all undelivered invoice
            using (var db = new xISPContext())
            {
                var invs = db.Invoices.Where(x => Statuses.Valid.Equals(x.Status) && x.AutoDeliver && x.DeliveredDate == null).ToList();

                foreach (var inv in invs)
                {
                    if (inv.IssuedDate.AddHours(SysConfig.Instance.InvoiceAutoDeliveryDelayHours) > DateTime.Now)
                    {
                        //too early to deliver
                        continue;
                    }

                    inv.Deliver(db);
                }
            }
            Logger.Info("DeliverAllInvoice", "end deliver all invoice");
        }

        #endregion
    }
}