using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class Plan
    {
        [NotMapped]
        public double MonthlyPriceGSTExclusive
        {
            get { return IsBusiness ? MonthlyPrice : MonthlyPrice / (1 + SysConfig.Instance.GST); }
        }

        [NotMapped]
        public double MonthlyPriceGSTInclusieve
        {
            get { return IsBusiness ? MonthlyPrice * (1 + SysConfig.Instance.GST) : MonthlyPrice; }
        }

        [NotMapped]
        public double MonthlyGST
        {
            get
            {
                return IsBusiness ? MonthlyPrice * SysConfig.Instance.GST :
                    MonthlyPrice * SysConfig.Instance.GST / (1 + SysConfig.Instance.GST);
            }
        }

        [NotMapped]
        public bool IsNaked
        {
            get
            {
                return (PstnCount == 0 && VoipCount == 0);
            }
        }

        public static IList<Plan> Get(xISPContext db)
        {
            return db.Plans.ToList();
        }

        public static Plan Get(xISPContext db, string id)
        {
            return db.Plans.SingleOrDefault(x => x.Id.Equals(id));
        }

        public static IList<Plan> Get(xISPContext db, bool isBusiness, string braodbandType = null)
        {
            Func<Plan, bool> condition;
            if (string.IsNullOrEmpty(braodbandType))
            {
                condition = x => x.IsBusiness == isBusiness && x.IsActive;

            }
            else {
                condition = x => x.IsBusiness == isBusiness && x.BroadbandType.Equals(braodbandType);

            }
            return db.Plans.Where(condition).OrderBy(x => x.DisplayPriority).ToList();
        }

        public Plan Save(xISPContext db)
        {
            db.Entry(this).State = EntityState.Modified;
            db.SaveChanges();
            return db.Plans.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        public Plan Create(xISPContext db)
        {
            this.Id = Guid.NewGuid().ToString();
            db.Plans.Add(this);
            db.SaveChanges();
            return db.Plans.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        public static bool Delete(xISPContext db, string id)
        {
            var p = db.Plans.SingleOrDefault(x => x.Id.Equals(id));
            if (p == null)
            {
                return false;
            }
            db.Plans.Remove(p);
            db.SaveChanges();
            p = db.Plans.SingleOrDefault(x => x.Id.Equals(id));
            return p == null;
        }

        public Product ToProduct(int prepayMonth)
        {
            //product
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                BasePriceGSTExclusive = MonthlyPriceGSTExclusive,
                NumberOfMonthPerCharge = 1,
                DiscountRate = 0,
                IsTermed = MonthsOfContract > 0,
                MonthsOfTerm = MonthsOfContract,
                IsOneOff = false,
                PrepayMonths = prepayMonth,
                AppliedDate = DateTime.Now
            };

            //service
            //broadband
            var broadband = new Service
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = product.Id,
                Type = Service.Types.BroadBand,
                SubType = BroadbandType,
                Status = Service.Statuses.Applied
            };
            product.Services.Add(broadband);

            //pstn
            for (int i = 0; i < PstnCount; i++)
            {
                var pstn = new Service
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = product.Id,
                    Type = Service.Types.Phone,
                    SubType = Service.PhoneSubTypes.PSTN,
                    Status = Service.Statuses.Applied
                };
                product.Services.Add(pstn);
            }

            //voip
            for (int i = 0; i < VoipCount; i++)
            {
                var voip = new Service
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = product.Id,
                    Type = Service.Types.Phone,
                    SubType = Service.PhoneSubTypes.VoIP,
                    Status = Service.Statuses.Applied
                };
                product.Services.Add(voip);
            }

            return product;
        }
    }
}
