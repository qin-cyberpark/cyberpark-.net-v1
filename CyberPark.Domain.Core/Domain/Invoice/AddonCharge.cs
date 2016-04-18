namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    public partial class AddonCharge
    {
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
                    ServiceId = srv.Id;
                    AccountId = srv.Product.AccountId;
                    return true;
                }
                else
                {
                    Warning = ExternalBillWarnings.Unmatched;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Warning = ex.Message;
                return false;
            }
        }
    }
}
