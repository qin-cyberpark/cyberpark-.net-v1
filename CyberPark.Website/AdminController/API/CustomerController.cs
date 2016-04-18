using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;
using System.Data.Entity;
using CyberPark.Website.ViewModels;
namespace CyberPark.Website.Controllers.API
{
    [Authorize(Roles = "Staff")]
    public class CustomerController : ApiController
    {
        private xISPContext _db;
        public CustomerController()
        {
            _db = new xISPContext();

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        //GET: /api/customer
        public ApiResult<IList<CustomerViewModels.CustomerListItem>> Get([FromUri] string customerId,
            [FromUri] string name, [FromUri] string accountId, [FromUri] string address,
            [FromUri] string asid, [FromUri] string pstn, [FromUri] string voip)
        {
            Func<Customer, bool> query = cus => cus.Id.ToString().Contains(customerId?.Trim() ?? "") // customer id
                                                 && cus.Name.ToLower().Contains(name?.Trim().ToLower() ?? "")                  // name
                                                 && cus.Accounts.Any(acc => acc.Id.ToString().Contains(accountId?.Trim() ?? ""))  //account id
                                                 && cus.Accounts.Any(acc => acc.Address.ToLower().Contains(address?.Trim().ToLower() ?? "")); //address
                                                 //&& cus.Accounts.Any(acc => acc.ServicePackages.Any(pkg => pkg.Services.Any(srv => (srv.BroadbandASID ?? "").Contains(asid?.Trim() ?? ""))))//asid
                                                 //&& cus.Accounts.Any(acc => acc.ServicePackages.Any(pkg => pkg.Services.Any(srv => (srv.PstnNumber ?? "").Contains(pstn?.Trim() ?? ""))));//pstn
                                                 //&& cus.Accounts.Any(acc => acc.ServicePackages.Any(pkg => pkg.Services.Any(srv => (srv.VoipNumber ?? "").Contains(voip?.Trim() ?? ""))));//voip

            var rslt = new ApiResult<IList<CustomerViewModels.CustomerListItem>>
            {
                Data = CustomerViewModels.CustomerListItem.Convert(Customer.Get(_db,query))
            };

            return rslt;
        }

        //GET: /api/customer/id
        public ApiResult<Customer> Get(int id)
        {
            return new ApiResult<Customer>
            {
                Data = Customer.GetById(_db,id)
            };
        }

        [HttpPut]
        public ApiResult<Customer> Update(Customer customer)
        {
            return new ApiResult<Customer>
            {
                Data = customer.Save(_db)
            };
        }


    }
}
