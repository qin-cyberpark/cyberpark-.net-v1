using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;
using CyberPark.Website.ViewModels;
namespace CyberPark.Website.Controllers.API
{
    public class PlanController : ApiController
    {
        private xISPContext _db;
        public PlanController()
        {
            _db = new xISPContext();

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        [Route("api/available-plan")]
        [HttpGet]
        public ApiResult<ApplicationViewModels.BroadbandAvailabilityViewModel> GetAvailable([FromUri]string address, [FromUri]bool isBusiness)
        {
            var availability = Utility.GetBroadbandAvailabilityByAdderss(address);
            if (availability == null)
            {
                return null;
            }

            var vm = new ApplicationViewModels.BroadbandAvailabilityViewModel
            {
                Address = availability.Address,
                ADSL = availability.ADSL,
                VDSL = availability.VDSL,
                UFB = availability.UFB,
                Active = availability.Active
            };

            if (vm.ADSL)
            {
                vm.ADSLs = Plan.Get(_db, isBusiness, Service.BroadbandSubTypes.ADSL);
            }
            if (vm.VDSL)
            {
                vm.VDSLs = Plan.Get(_db, isBusiness, Service.BroadbandSubTypes.VDSL);
            }
            if (vm.UFB)
            {
                vm.UFBs = Plan.Get(_db, isBusiness, Service.BroadbandSubTypes.UFB);
            }

            var rslt = new ApiResult<ApplicationViewModels.BroadbandAvailabilityViewModel>
            {
                Data = vm
            };

            return rslt;
        }
    }
}
