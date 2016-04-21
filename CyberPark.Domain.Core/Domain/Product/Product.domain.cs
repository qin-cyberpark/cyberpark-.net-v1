using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Data.Entity;

namespace CyberPark.Domain.Core
{
    public partial class Product
    {
        #region properties
        //[NotMapped]
        //public string Status {
        //    get
        //    {
        //        var srv = Services.FirstOrDefault(x => Service.Types.BroadBand.Equals(x.Type));
        //        if (srv != null)
        //        {
        //            return srv.Status;
        //        }

        //        srv = Services.FirstOrDefault(x => Service.Types.Phone.Equals(x.Type));
        //        return srv?.Status;
        //    }
        //}

        [NotMapped]
        public double PriceGSTExclusive
        {
            get
            {
                return Math.Round(BasePriceGSTExclusive * (1 - DiscountRate / 100),2);
            }
        }
        [NotMapped]
        public double PriceGSTInclusive
        {
            get
            {
                return Math.Round(PriceGSTExclusive * (1 + SysConfig.Instance.GST),2);
            }
        }
        [NotMapped]
        public bool IsPackage
        {
            get
            {
                return Services.Count + UsageOffers.Count > 1;
            }
        }
        [NotMapped]
        public bool IsSingleService
        {
            get
            {
                return !IsPackage && Services.Count == 1;
            }
        }
        [NotMapped]
        public bool IsSingleUsageOffer
        {
            get
            {
                return !IsPackage && UsageOffers.Count == 1;
            }
        }
        [NotMapped]
        public bool IsMonthly
        {
            get
            {
                return !IsOneOff;
            }
        }

        [NotMapped]
        public DateTime? TermEndDate
        {
            get
            {
                if (TermStartDate == null || MonthsOfTerm == null)
                {
                    return null;
                }

                return TermStartDate.Value.AddMonths(MonthsOfTerm.Value);
            }
        }
        [NotMapped]
        public string TermDescription
        {
            get
            {
                if (!IsTermed)
                {
                    return "No-Term";
                }
                if (TermStartDate == null || MonthsOfTerm == null)
                {
                    return "Unknown";
                }
                return string.Format("{0:ddMMM yyyy} to {1:ddMMM yyyy}", TermStartDate, TermEndDate);
            }
        }
        #endregion

        #region product charge
        /// <summary>
        /// 1. status = using
        /// 2. one-off(not monthly) and has charged
        /// 3. monthly has current charge to
        /// 4. current charge to - now < advance day
        /// </summary>
        [NotMapped]
        private bool Chargeable
        {
            get
            {
                //not using
                if (!Status.Equals(Service.Statuses.InService))
                {
                    return false;
                }

                //have charge to date is more an one month from current date
                if(ChargedToDate != null && DateTime.Today.AddMonths(1) < ChargedToDate)
                {
                    return false;
                }

                //one-off
                if (IsOneOff)
                {
                    if (HasOneOffCharged ?? false)
                    {
                        //has charged
                        return false;
                    }
                    else if (DateTime.Today == OneOffChargeDate.Value.Date)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }


                if (ChargedToDate != null)
                {
                    if ((Account.Type.Equals(Account.Types.Personal) && (DateTime.Today - ChargedToDate.Value.Date).Days > 0)
                        && (Account.Type.Equals(Account.Types.Business) && (DateTime.Today - ChargedToDate.Value.Date).Days > 30))
                    {
                        //personal - last month was not charged 
                        throw new ProductChargeException(string.Format("product {0}: last month was not charged", Name), AccountId);
                    }
                }

                return true;
            }
        }

        public double CalculateChargeAmount()
        {
            if (ChargedToDate == null && Account.InvoicePeriodType.Equals(Account.InvoicePeriodTypes.CalendarMonth))
            {
                //first CalendarMonth Mode charege = half month + per charge months
                var daysInMonth = DateTime.DaysInMonth(ServiceGivenDate.Value.Month, ServiceGivenDate.Value.Day);
                var remainDays = daysInMonth - ServiceGivenDate.Value.Day + 1;
                return BasePriceGSTExclusive * (1 + remainDays / daysInMonth);
            }
            else
            {
                return BasePriceGSTExclusive;
            }
        }

        public DateTime CalculateChargeToDate()
        {
            if (ChargedToDate == null)
            {
                //first charge
                if (Account.InvoicePeriodType.Equals(Account.InvoicePeriodTypes.CalendarMonth))
                {
                    //CalendarMonth Mode = half month + per charge months
                    var nxtMonth = DateTime.Now.AddMonths(NumberOfMonthPerCharge);
                    return new DateTime(nxtMonth.Year, nxtMonth.Month, DateTime.DaysInMonth(nxtMonth.Year, nxtMonth.Month));
                }
                else
                {
                    //ServiceGivenDay = Service Given Date + per charge months
                    return ServiceGivenDate.Value.AddDays(-1).AddMonths(NumberOfMonthPerCharge);
                }
            }
            else
            {
                var nxtMonth = ChargedToDate.Value.AddMonths(NumberOfMonthPerCharge);
                if (Account.InvoicePeriodType.Equals(Account.InvoicePeriodTypes.CalendarMonth))
                {
                    //CalendarMonth mode = last day of next charge month
                    return new DateTime(nxtMonth.Year, nxtMonth.Month, DateTime.DaysInMonth(nxtMonth.Year, nxtMonth.Month));
                }
                else
                {
                    //ServiceGivenDay + per charge months
                    if (DateTime.DaysInMonth(nxtMonth.Year, nxtMonth.Month) < ServiceGivenDate.Value.Day)
                    {
                        return new DateTime(nxtMonth.Year, nxtMonth.Month, DateTime.DaysInMonth(nxtMonth.Year, nxtMonth.Month));
                    }
                    else
                    {
                        return new DateTime(nxtMonth.Year, nxtMonth.Month, ServiceGivenDate.Value.Day).AddDays(-1);
                    }

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Charge()
        {
            /*
            1.create charge record
            2.set charge to date
            3.set charged (one-off)
            */
            using (var db = new xISPContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    Charge(db);
                    trans.Commit();
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Charge(xISPContext db)
        {
            /*
            1.create charge record
            2.set charge to date
            3.set charged (one-off)
            */

            if (Chargeable)
            {
                //create charge record
                var chrgRec = new ProductCharge(AccountId, Id);
                if (IsMonthly)
                {
                    chrgRec.PreviousProductChargedToDate = ChargedToDate == null ? DateTime.MinValue : ChargedToDate.Value.Date;
                    chrgRec.CurrentProductChargedToDate = CalculateChargeToDate();

                    chrgRec.AmountGSTExclusive = CalculateChargeAmount();

                    //set current charge to date
                    ChargedToDate = chrgRec.CurrentProductChargedToDate;
                }
                else
                {
                    //one-off
                    chrgRec.AmountGSTExclusive = BasePriceGSTExclusive;
                    HasOneOffCharged = true;
                }

                db.Entry(this).State = EntityState.Modified;
                db.ProductCharges.Add(chrgRec);


                db.SaveChanges();

            }

            return true;
        }
        /// <summary>
        /// do all accouts' service charge
        /// </summary>
        //public static void ChargeAll()
        //{
        //    IList<Product> pkgs;
        //    using (var db = new xISPContext())
        //    {
        //        pkgs = db.Products.Include(x => x.Account).Where(x => x.Status.Equals(Service.Statuses.InService)
        //                             && (!x.IsOneOff || !(x.HasOneOffCharged ?? false))).ToList();
        //    }
        //    foreach (var p in pkgs)
        //    {
        //        try
        //        {
        //            p.Charge();
        //        }
        //        catch (Exception ex)
        //        {
        //            Warning.WriteAsync("ProductCharge", "Charge", "failed to calculate service charging," + ex.Message, accountId: p.AccountId);
        //        }
        //    }
        //}
        #endregion

        #region data operates
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="accountId"></param>
        /// <param name="isMonthly">NULL = ALL</param>
        /// <returns></returns>
        public static IList<Product> GetProductsByAccountId(xISPContext db, int accountId, bool? isMonthly = null)
        {
            Func<Product, bool> query;
            if (isMonthly == null)
            {
                query = x => x.AccountId == accountId;
            }
            else
            {
                query = x => x.AccountId == accountId && x.IsMonthly == isMonthly;
            }

            return db.Products.Include(x => x.Services).Include(x => x.UsageOffers)
                                .Where(query).ToList();
        }
        public static IList<Product> GetMonthlyProductsByAccountId(xISPContext db, int accountId)
        {
            return GetProductsByAccountId(db, accountId, true);
        }
        public static IList<Product> GetOneOffProductsByAccountId(xISPContext db, int accountId)
        {
            return GetProductsByAccountId(db, accountId, false);
        }

        public static Product Get(xISPContext db, string id)
        {
            return db.Products.Include(x => x.Services)
                              .Include(x => x.Account)
                              .Where(x => x.Id.Equals(id)).FirstOrDefault();
        }

        public static Product Create(xISPContext db, Product product)
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = Guid.NewGuid().ToString();
            }

            db.Products.Add(product);
            db.SaveChanges();

            return product;
        }

        public Product Save(xISPContext db)
        {
            db.Entry(this).State = EntityState.Modified;
            db.SaveChanges();
            return db.Products.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        #endregion
    }
}
