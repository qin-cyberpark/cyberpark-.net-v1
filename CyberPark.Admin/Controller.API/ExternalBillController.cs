using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;
using CyberPark.Website.ViewModels;
using CyberPark.Website.Models;
namespace CyberPark.Website.Controllers.API
{
    [Authorize(Roles = "Staff")]
    public class ExternalBillController : ApiController
    {
        private xISPContext _db;
        public ExternalBillController()
        {
            _db = new xISPContext();

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        //PUT: /api/externalbill/match
        [HttpPut]
        [Route("api/externalbill/match")]
        public ApiResult<int> Match(ExternalBillViewModels.BillMatchIgnoreModel model)
        {
            var accoutId = ExternalBill.MatchAccount(_db, model.BillId, model.Number, model.isCall);
            return new ApiResult<int>
            {
                Success = accoutId != 0,
                Data = accoutId
            };
        }

        //PUT: /api/externalbill/ignore
        [HttpPut]
        [Route("api/externalbill/ignore")]
        public ApiResult<bool> Ignore(ExternalBillViewModels.BillMatchIgnoreModel model)
        {
            var result = ExternalBill.Ignore(_db, model.BillId, model.Number, model.isCall, xISPUser.CurrentUserId);
            return new ApiResult<bool>
            {
                Success = result,
                Data = result
            };
        }

        //get bill unmatched info
        public ApiResult<ExternalBillViewModels.BillViewModel> Get(string id){
            return new ApiResult<ExternalBillViewModels.BillViewModel>
            {
                Data = new ExternalBillViewModels.BillViewModel(ExternalBill.Get(id))
            };
        }
    }
}
