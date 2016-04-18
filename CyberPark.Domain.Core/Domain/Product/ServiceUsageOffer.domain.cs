namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Data.Entity;

    public partial class ServiceUsageOffer
    {
        [NotMapped]
        public bool Unlimited
        {
            get
            {
                return Minutes == -1;
            }
        }

        public string Description
        {
            get
            {
                if (!Local && !Mobile && !National && string.IsNullOrEmpty(CallingRegionId))
                {
                    return null;
                }

                if (ServiceType.Equals(Service.Types.Phone))
                {
                    return string.Format("{0} mins to {1}{2}{3}{4}[{5}]",
                            Minutes,
                            CallingRegion?.Description ?? "Selected Area",
                            Local ? "+Local" : "",
                            National ? "+National" : "",
                            Mobile ? "+Mobile" : "",
                            ServiceSubType
                        );
                }
                else
                {
                    return ServiceType;
                }
            }
        }


        public bool IsApplicable(CallingRecord callRec)
        {
            //
            var regsions = CallingRegion?.Details;

            //type
            if (callRec.Service == null || !callRec.Service.Type.Equals(ServiceType)
                || !callRec.Service.SubType.Equals(ServiceSubType))
            {
                return false;
            }

            //local
            if ((callRec.Type.Equals(CallingRecord.Types.Local) && Local) ||
                (callRec.Type.Equals(CallingRecord.Types.National) && National) ||
                (callRec.Type.Equals(CallingRecord.Types.Mobile) && Mobile))
            {
                return true;
            }

            //internation
            if (callRec.Type.Equals(CallingRecord.Types.International) && CallingRegion?.Details != null)
            {
                foreach (var country in CallingRegion.Details)
                {
                    if (callRec.AreaName.ToUpper().Contains(country.CountryName.ToUpper()) &&
                        country.IncludeMobile ? true : !callRec.IsMobile)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// create
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public ServiceUsageOffer Create(xISPContext db, int userId)
        {
            this.Id = Guid.NewGuid().ToString();
            OperatorId = userId;
            db.ServiceUsageOffers.Add(this);
            db.SaveChanges();
            return db.ServiceUsageOffers.SingleOrDefault(x => x.Id.Equals(this.Id));
        }


        /// <summary>
        /// save
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public ServiceUsageOffer Save(xISPContext db, int userId)
        {
            OperatorId = userId;
            db.Entry(this).State = EntityState.Modified;
            db.SaveChanges();
            return db.ServiceUsageOffers.SingleOrDefault(x => x.Id.Equals(this.Id));
        }


        /// <summary>
        /// delete
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public ServiceUsageOffer Delete(xISPContext db, int userId)
        {
            IsDeleted = true;
            return Save(db, userId);
        }

        public static bool Delete(xISPContext db, string id, int userId)
        {
            var o = db.ServiceUsageOffers.SingleOrDefault(x => x.Id.Equals(id));
            if (o == null)
            {
                return false;
            }

            var newS = o.Delete(db, userId);
            return newS.IsDeleted;
        }

    }
}
