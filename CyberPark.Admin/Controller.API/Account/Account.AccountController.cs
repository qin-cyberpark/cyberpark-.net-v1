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
            [FromUri] string asid, [FromUri] string pstn, [FromUri] string voip, [FromUri] string status)
        {

            Func<Account, bool> query = acct => acct.CustomerId.ToString().Contains(customerId?.Trim() ?? "") // customer id
                                                  && acct.Name.ToLower().Contains(name?.Trim().ToLower() ?? "") // name
                                                  && acct.Id.ToString().Contains(accountId?.Trim() ?? "")  //account id
                                                  && acct.Address.ToLower().Contains(address?.Trim().ToLower() ?? "");//address
                                                  //&& acct.Products.Any(p => p.Services.Any(s => (s.IdentityNumber ?? "").Contains(asid?.Trim() ?? "") && Service.Types.BroadBand.Equals(s.Type)))//asid
                                                  //&& acct.Products.Any(p => p.Services.Any(s => (s.IdentityNumber ?? "").Contains(pstn?.Trim() ?? "") && Service.PhoneSubTypes.PSTN.Equals(s.SubType)));//pstn
                                                  //&& acct.Products.Any(p => p.Services.Any(s => (s.IdentityNumber ?? "").Contains(voip?.Trim() ?? "") && Service.PhoneSubTypes.VoIP.Equals(s.SubType)));//voip

            var accts = Account.Get(_db, query, true);
            if (!string.IsNullOrEmpty(status))
            {
                accts = accts.Where(x => x.Products.Any(p=>p.Status.Equals(status))).ToList();
            }

            var rslt = new ApiResult<IList<Account>>
            {
                Data = accts
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