using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;
using System.Xml;
using CyberPark.Website.ViewModels;
namespace CyberPark.Website.Controllers.API
{
    public class UtilityController : ApiController
    {
        [Route("api/broadband-availability")]
        [HttpGet]
        public ApiResult<Utility.BroadbandAvailability> AddressCheck([FromUri]string address)
        {
            return new ApiResult<Utility.BroadbandAvailability>
            {
                Data = Utility.GetBroadbandAvailabilityByAdderss(address)
            };
        }
    }
}
