namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity;

    public partial class ServicePrice
    {
        public static IList<ServicePrice> Get(xISPContext db)
        {
            return db.ServicePrices.ToList();
        }

        public static IList<ServicePrice> Get(xISPContext db, Func<ServicePrice, bool> condition)
        {
            return db.ServicePrices.Where(condition).ToList();
        }

        public ServicePrice Save(xISPContext db)
        {
            db.ServicePrices.Attach(this);
            db.Entry(this).State = EntityState.Modified;
            db.SaveChanges();
            return db.ServicePrices.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        public ServicePrice Create(xISPContext db)
        {
            this.Id = Guid.NewGuid().ToString();
            db.ServicePrices.Add(this);
            db.SaveChanges();
            return db.ServicePrices.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        public static bool Delete(xISPContext db, string id)
        {
            var p = db.ServicePrices.SingleOrDefault(x => x.Id.Equals(id));
            if (p == null)
            {
                return false;
            }
            db.ServicePrices.Remove(p);
            db.SaveChanges();
            p = db.ServicePrices.SingleOrDefault(x => x.Id.Equals(id));
            return p == null;
        }

        //public static Plan SetActivity(string planId, bool isActive)
        //{
        //    using(var db = new xISPContext())
        //    {
        //        db.Plans.FirstOrDefault(x => x.Id.Equals(planId)).IsActive = isActive;
        //        db.SaveChanges();
        //        return db.Plans.FirstOrDefault(x => x.Id.Equals(planId));
        //    }
        //}

        public static IList<ServicePrice> GetDefaultService(xISPContext db, bool isBusiness, bool adsl = true, bool vdsl = true, bool ufb = true)
        {
            IList<ServicePrice> result = new List<ServicePrice>();

            if (adsl)
            {
                var srv = db.ServicePrices.OrderBy(x => x.PriceGSTExclusive).FirstOrDefault(x => x.IsActive && x.ServiceSubType.Equals(Service.BroadbandSubTypes.ADSL)
                                          && x.IsBusiness == isBusiness);
                if (srv != null)
                {
                    result.Add(srv);
                }
            }

            if (vdsl)
            {
                var srv = db.ServicePrices.OrderBy(x => x.PriceGSTExclusive).FirstOrDefault(x => x.IsActive && x.ServiceSubType.Equals(Service.BroadbandSubTypes.VDSL)
                                          && x.IsBusiness == isBusiness);
                if (srv != null)
                {
                    result.Add(srv);
                }
            }

            if (ufb)
            {
                var srv = db.ServicePrices.OrderBy(x => x.PriceGSTExclusive).FirstOrDefault(x => x.IsActive && x.ServiceSubType.Equals(Service.BroadbandSubTypes.UFB)
                                          && x.IsBusiness == isBusiness);
                if (srv != null)
                {
                    result.Add(srv);
                }
            }

            return result;
        }

        public static IList<ServicePrice> GetServicesByType(xISPContext db, bool isBusiness, string type, string subType = null)
        {
            Func<ServicePrice, bool> condition;
            if (!string.IsNullOrEmpty(subType))
            {
                condition = x => x.IsActive && x.IsBusiness == isBusiness && x.ServiceType.Equals(type) && x.ServiceSubType.Equals(subType);
            }
            else
            {
                condition = x => x.IsActive && x.IsBusiness == isBusiness && x.ServiceType.Equals(type);
            }

            return Get(db, condition);
        }
    }
}
