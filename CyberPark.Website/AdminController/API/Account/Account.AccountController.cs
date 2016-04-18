using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Website.ViewModels;
using CyberPark.Domain.Core;
namespace CyberPark.Website.Controllers.API
{
    public partial class AccountController : ApiController
    {
        private xISPContext _db;
        public AccountController()
        {
            _db = new xISPContext();

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        //GET: /api/customer
        public ApiResult<IList<Account>> Get([FromUri] string customerId, [FromUri] string name, [FromUri] string accountId, [FromUri] string address,
            [FromUri] string asid, [FromUri] string pstn, [FromUri] string voip)
        {

            Func<Account, bool> query = acct => acct.CustomerId.ToString().Contains(customerId?.Trim() ?? "") // customer id
                                                  && acct.Name.ToLower().Contains(name?.Trim().ToLower() ?? "")                  // name
                                                  && acct.Id.ToString().Contains(accountId?.Trim() ?? "")  //account id
                                                  && acct.Address.ToLower().Contains(address?.Trim().ToLower() ?? ""); //address
                                                                                                                       //&& cus.Accounts.Any(acc => acc.ServicePackages.Any(pkg => pkg.Services.Any(srv => (srv.BroadbandASID ?? "").Contains(asid?.Trim() ?? ""))))//asid
                                                                                                                       //&& cus.Accounts.Any(acc => acc.ServicePackages.Any(pkg => pkg.Services.Any(srv => (srv.PstnNumber ?? "").Contains(pstn?.Trim() ?? ""))));//pstn
                                                                                                                       //&& cus.Accounts.Any(acc => acc.ServicePackages.Any(pkg => pkg.Services.Any(srv => (srv.VoipNumber ?? "").Contains(voip?.Trim() ?? ""))));//voip

            var rslt = new ApiResult<IList<Account>>
            {
                Data = Account.Get(_db, query)
            };

            return rslt;
        }


        //GET: /api/account/id
        public ApiResult<Account> Get(int id)
        {
            return new ApiResult<Account>
            {
                Data = Account.Get(_db, id)
            };
        }

        [HttpPut]
        public ApiResult<Account> Update(Account acct)
        {
            return new ApiResult<Account>
            {
                Data = acct.Save(_db)
            };
        }

 
    }
}