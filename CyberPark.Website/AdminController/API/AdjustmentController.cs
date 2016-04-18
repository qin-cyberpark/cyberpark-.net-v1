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
    public class AdjustmentController : ApiController
    {
        private xISPContext _db;
        public AdjustmentController()
        {
            _db = new xISPContext();

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPut]
        public ApiResult<Adjustment> Update(Adjustment adj)
        {
            return new ApiResult<Adjustment>
            {
                //Data = adj.Save(_db, xISPUser.CurrentUserId)
            };
        }

        //POST: /api/service
        [HttpPost]
        public ApiResult<Adjustment> Create(Adjustment adj)
        {
            return new ApiResult<Adjustment>
            {
                Data = adj.Create(_db, xISPUser.CurrentUserId)
            };
        }

        [HttpDelete]
        public ApiResult<bool> Delete(string id)
        {
            var adj = Adjustment.GetById(_db, id);
            if (adj == null)
            {
                return new ApiResult<bool>
                {
                    Success = false,
                    Message = string.Format("Adjustment {0} is not existing", id)
                };
            }

            return new ApiResult<bool>
            {
                Data = adj.Delete(_db, xISPUser.CurrentUserId)
            };
        }
    }
}