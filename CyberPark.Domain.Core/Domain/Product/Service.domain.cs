namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Data.Entity;

    public partial class Service
    {
        #region constant
        public class Types
        {
            private Types()
            {

            }
            public const string BroadBand = "Broadband";
            public const string Phone = "Phone";
            public const string AddOn = "AddOn";
            public const string Misellaneous = "Misc";
        }
        public class BroadbandSubTypes
        {
            private BroadbandSubTypes()
            {

            }
            public const string ADSL = "ADSL";
            public const string VDSL = "VDSL";
            public const string UFB = "UFB";
        }
        public class PhoneSubTypes
        {
            private PhoneSubTypes()
            {

            }
            public const string PSTN = "PSTN";
            public const string VoIP = "VoIP";
            public const string Fax = "Fax";
        }

        public class MisellaneousSubTypes
        {
            private MisellaneousSubTypes()
            {

            }
            public const string NewConnection = "NewConnection";
        }
      
        #endregion

        [NotMapped]
        public string Description
        {
            get
            {
                switch (Type)
                {
                    case Types.BroadBand:
                        return string.Format("{0}({1}) {2}", Type, SubType, IdentityNumber);
                    case Types.Phone:
                        return string.Format("{0}({1})", SubType.Equals(PhoneSubTypes.PSTN) ? "Land Line" :
                                                         SubType.Equals(PhoneSubTypes.VoIP) ? "VoIP Phone" :
                                                         SubType.Equals(PhoneSubTypes.Fax) ? "Fax" : SubType, IdentityNumber);
                    default:
                        return string.Format("{0}", Type, SubType);
                }
            }
        }

        [NotMapped]
        public IList<string> PossibleNextStatus { get; set; }

        /// <summary>
        /// create
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public Service Create(xISPContext db, int userId)
        {
            this.Id = Guid.NewGuid().ToString();
            this.OperatorId = userId;
            db.Services.Add(this);
            db.SaveChanges();
            return db.Services.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        /// <summary>
        /// save
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public Service Save(xISPContext db, int userId)
        {
            db.Entry(this).State = EntityState.Modified;
            OperatorId = userId;
            db.SaveChanges();
            return db.Services.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        public Service Delete(xISPContext db, int userId)
        {
            IsDeleted = true;
            return Save(db, userId);
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool Delete(xISPContext db, string id, int userId)
        {
            var s = db.Services.SingleOrDefault(x => x.Id.Equals(id));
            if (s == null)
            {
                return false;
            }
            var newS = s.Delete(db, userId);
            return newS.IsDeleted;
        }
    }
}