namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

     public partial class CallingRecord
    {
   
        [NotMapped]
        public double ChargeIncludeGTS
        {
            get
            {
                return Charge * 1.15D;
            }
        }

        [NotMapped]
        public double? ActualChargeIncludeGTS
        {
            get
            {
                return ActualCharge * 1.15D;
            }
        }
        public static int ConvertSecondDurationToMinute(int sec)
        {
            return sec / 60 + (sec % 60 > 0 ? 1 : 0);
        }

        public bool MatchAccount(xISPContext db)
        {
            try
            {
                var srv = db.Services.Include(x => x.Product)
                                    .SingleOrDefault(s => s.IdentityNumber.Equals(OriNumber)
                                            || OriNumber.Equals("0" + s.IdentityNumber) 
                                            || OriNumber.Equals("64" + s.IdentityNumber));
                if (srv != null)
                {
                    if (!srv.Status.Equals(Service.Statuses.InService))
                    {
                        Warning = ExternalBillWarnings.ServiceNotInUsing;
                        return false;
                    }

                    ServiceId = srv.Id;
                    PhoneType = srv.SubType;
                    //AccountId = srv.ServicePackage.AccountId;
                    return true;
                }
                else
                {
                    Warning = ExternalBillWarnings.Unmatched;
                    return false;
                }
            }
            catch(Exception ex)
            {
                Warning = ex.Message;
                return false;
            }
        }
    }
}
