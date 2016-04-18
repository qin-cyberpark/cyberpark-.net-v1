using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;
using CyberPark.Website.ViewModels;
namespace CyberPark.Website.Controllers.API
{
    [Authorize(Roles = "Staff")]
    public class ServicePriceController : ApiController
    {
        private xISPContext _db;
        public ServicePriceController()
        {
            _db = new xISPContext();

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


        //GET: /api/service
        public ApiResult<IList<ServicePrice>> Get()
        {
            var rslt = new ApiResult<IList<ServicePrice>> {
                Data = ServicePrice.Get(_db)
            };

            return rslt;
        }

        //PUT: /api/plan
        [HttpPut]
        public ApiResult<ServicePrice> Update(ServicePrice price)
        {
            return new ApiResult<ServicePrice>
            {
                Data = price.Save(_db)
            };
        }

        //POST: /api/service
        [HttpPost]
        public ApiResult<ServicePrice> Create(ServicePrice price)
        {
            return new ApiResult<ServicePrice>
            {
                Data = price.Create(_db)
            };
        }

        //DELETE: /api/service/id
        [HttpDelete]
        public ApiResult<bool> Remove(string id)
        {
            return new ApiResult<bool>
            {
                Data = ServicePrice.Delete(_db,id)
            };
        }

        //[AllowAnonymous]
        //[Route("api/available-broadband-service")]
        //[HttpGet]
        //public ApiResult<ApplicationViewModels.BroadbandAvailabilityViewModel> GetAvailable([FromUri]string address, [FromUri]bool isBusiness)
        //{
        //    var availability = Utility.GetBroadbandAvailabilityByAdderss(address);
        //    if (availability == null)
        //    {
        //        return null;
        //    }

        //    var vm = new ApplicationViewModels.BroadbandAvailabilityViewModel
        //    {
        //        Address = availability.Address,
        //        ADSL = availability.ADSL,
        //        VDSL = availability.VDSL,
        //        UFB = availability.UFB,
        //        Active = availability.Active
        //    };

        //   vm.AvailableServices = ServicePrice.GetDefaultService(_db, isBusiness, availability.ADSL, availability.VDSL, availability.UFB);
            
        //    var rslt = new ApiResult<ApplicationViewModels.BroadbandAvailabilityViewModel>
        //    {
        //        Data = vm
        //    };

        //    return rslt;
        //}
    }
}
