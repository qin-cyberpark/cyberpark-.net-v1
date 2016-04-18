using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;
namespace CyberPark.Website.Controllers.API
{
    [Authorize(Roles = "Staff")]
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


        //GET: /api/plan
        public ApiResult<IList<Plan>> Get()
        {
            var rslt = new ApiResult<IList<Plan>> {
                Data = Plan.Get(_db)
            };

            return rslt;
        }

        //PUT: /api/plan
        [HttpPut]
        public ApiResult<Plan> Update(Plan plan)
        {
            return new ApiResult<Plan>
            {
                Data = plan.Save(_db)
            };
        }

        //POST: /api/plan
        [HttpPost]
        public ApiResult<Plan> Create(Plan plan)
        {
            return new ApiResult<Plan>
            {
                Data = plan.Create(_db)
            };
        }

        //DELETE: /api/plan/id
        [HttpDelete]
        public ApiResult<bool> Remove(string id)
        {
            return new ApiResult<bool>
            {
                Data = Plan.Delete(_db,id)
            };
        }
    }
}
