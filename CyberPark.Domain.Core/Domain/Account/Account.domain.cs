namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.Serialization;

    public partial class Account
    {
        public class Types
        {
            private Types()
            {

            }
            public const string Personal = "Personal";
            public const string Business = "Business";
        }

        public class InvoicePeriodTypes
        {
            private InvoicePeriodTypes()
            {

            }
            public const string CalendarMonth = "CalendarMonth";
            public const string ServiceGivenDay = "ServiceGivenDay";
        }
        #region properties
        [NotMapped]
        public string Name
        {
            get
            {
                if (IsBusiness && !string.IsNullOrEmpty(OrganizationName))
                {
                    return OrganizationName;
                }
                else
                {
                    if (!string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName))
                    {
                        return string.Format("{0} {1}", FirstName, LastName);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        [NotMapped]
        public bool IsBusiness
        {
            get
            {
                return Types.Business.Equals(Type);
            }
        }

        [NotMapped]
        private Invoice LastInvoice
        {
            get
            {
                if (Invoices.Count == 0)
                {
                    using (var db = new xISPContext())
                    {
                        db.Accounts.Attach(this);
                        db.Entry(this).Collection(x => x.Invoices).Load();
                    }

                }
                return Invoices.Where(x => !x.Status.Equals(Invoice.Statuses.Void)).OrderByDescending(x => x.IssuedDate).FirstOrDefault();
            }
        }

        //[NotMapped]
        //public string Status
        //{
        //    get
        //    {
        //        string status = null;

        //        foreach (var p in Products)
        //        {
        //            foreach(var s in p.Services)
        //            {
        //                if (Service.Types.BroadBand.Equals(s.Type))
        //                {
        //                    return s.Status;
        //                }

        //                if (Service.Types.Phone.Equals(s.Type) && string.IsNullOrEmpty(status))
        //                {
        //                    status = s.Status;
        //                }
        //            }
        //        }

        //        return status;
        //    }
        //}

        private DateTime? GetBroadbandServiceGivenDate(xISPContext db)
        {
            if (Products.Count == 0)
            {

                db.Accounts.Attach(this);
                db.Entry(this).Collection(x => x.Products).Load();
                foreach (var product in Products)
                {
                    db.Entry(product).Collection(x => x.Services).Load();
                }
            }

            foreach (var product in Products)
            {
                foreach (var srv in product.Services)
                {
                    if (Service.Types.BroadBand.Equals(srv.Type))
                    {
                        return product.ServiceGivenDate;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public bool IsLastInvoice(int year, int month)
        {
            var lstInv = LastInvoice;
            if (lstInv != null)
            {
                return lstInv.Year == year && lstInv.Month == month;
            }
            else
            {
                return false;
            }
        }


        #endregion
        #region operate
        public static Account Get(xISPContext db, int accountId, bool includeService = false)
        {
            if (!includeService)
            {
                return db.Accounts.SingleOrDefault(x => x.Id == accountId);
            }
            else
            {
                return db.Accounts.Include(x => x.Products)
                 .Include(x => x.Products.Select(y => y.Services))
                 .Include(x => x.Invoices)
                 .SingleOrDefault(x => x.Id == accountId);
            }
        }

        public static IList<Account> Get(xISPContext db, bool includeService = false)
        {
            if (!includeService)
            {
                return db.Accounts.ToList();
            }
            else
            {
                return db.Accounts.Include(x => x.Products)
                 .Include(x => x.Products.Select(y => y.Services))
                 .Include(x => x.Invoices).ToList();
            }
        }

        public static IList<Account> Get(xISPContext db, Func<Account, bool> query, bool includeService = false)
        {
            if (!includeService)
            {
                return db.Accounts.Where(query).OrderBy(x => x.Id).ToList();
            }
            else
            {
                return db.Accounts.Include(x => x.Products)
                 .Include(x => x.Products.Select(y => y.Services))
                 .Include(x => x.Invoices)
                 .Where(query).OrderBy(x => x.Id).ToList();
            }
           
        }

        public Account Save(xISPContext db)
        {
            db.Entry(this).State = EntityState.Modified;
            db.SaveChanges();
            return db.Accounts.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        public static Account Create(xISPContext db, Account account)
        {
            //get max id
            if (account.Id == 0)
            {
                account.Id = db.Accounts.Max(x => x.Id) + 1;
            }

            db.Accounts.Add(account);
            db.SaveChanges();

            return account;
        }
        #endregion


        public static Account CreateApplication(xISPContext db, Account account, Plan plan, int prepayMonth)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                //create account
                account.InvoicePeriodType = account.IsBusiness ? InvoicePeriodTypes.CalendarMonth : InvoicePeriodTypes.ServiceGivenDay;
                var nxtInvDate = DateTime.Today.AddMonths(1).AddDays(-1);
                if (account.IsBusiness)
                {
                    nxtInvDate = new DateTime(nxtInvDate.Year, nxtInvDate.Month, SysConfig.Instance.BusinessInvoiceIssueDay);
                }
                account.NextInvoiceIssueDate = nxtInvDate;
                account.IsActive = true;
                account = Create(db, account);

                //create product
                var product = plan.ToProduct(prepayMonth);
                account.Products.Add(product);

                db.SaveChanges();
                trans.Commit();
                return account;
            }
        }
    }
}
