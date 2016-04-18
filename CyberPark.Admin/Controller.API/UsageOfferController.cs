using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Website.ViewModels;
using CyberPark.Domain.Core;
using CyberPark.Website.Models;

namespace CyberPark.Website.Controllers.API
{
    [Authorize(Roles = "Staff")]
    public class UsageOfferController : ApiController
    {
        private xISPContext _db;
        public UsageOfferController()
        {
            _db = new xISPContext();

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPut]
        public ApiResult<ServiceUsageOffer> Update(ServiceUsageOffer offer)
        {
            return new ApiResult<ServiceUsageOffer>
            {
                Data = offer.Save(_db, xISPUser.CurrentUserId)
            };
        }

        //POST: /api/service
        [HttpPost]
        public ApiResult<ServiceUsageOffer> Create(ServiceUsageOffer offer)
        {
            return new ApiResult<ServiceUsageOffer>
            {
                Data = offer.Create(_db, xISPUser.CurrentUserId)
            };
        }

        [HttpDelete]
        public ApiResult<bool> Delete(string id)
        {
            return new ApiResult<bool>
            {
                Data = ServiceUsageOffer.Delete(_db, id, xISPUser.CurrentUserId)
            };
        }
    }
}
