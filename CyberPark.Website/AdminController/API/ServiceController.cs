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
    public class ServiceController : ApiController
    {
        private xISPContext _db;
        public ServiceController()
        {
            _db = new xISPContext();

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPut]
        public ApiResult<Service> Update(Service service)
        {
            return new ApiResult<Service>
            {
                Data = service.Save(_db, xISPUser.CurrentUserId)
            };
        }

        //POST: /api/service
        [HttpPost]
        public ApiResult<Service> Create(Service service)
        {
            return new ApiResult<Service>
            {
                Data = service.Create(_db, xISPUser.CurrentUserId)
            };
        }

        [HttpDelete]
        public ApiResult<bool> Delete(string id)
        {
            return new ApiResult<bool>
            {
                Data = Service.Delete(_db, id, xISPUser.CurrentUserId)
            };
        }
    }
}
