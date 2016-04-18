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
    public class TransactionController : ApiController
    {
        private xISPContext _db;
        public TransactionController()
        {
            _db = new xISPContext();

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPut]
        public ApiResult<Transaction> Update(Transaction trans)
        {
            return new ApiResult<Transaction>
            {
                //Data = trans.Save(_db, xISPUser.CurrentUserId)
            };
        }

        //POST: /api/service
        [HttpPost]
        public ApiResult<Transaction> Create(Transaction trans)
        {
            return new ApiResult<Transaction>
            {
                Data = trans.Create(_db, xISPUser.CurrentUserId)
            };
        }

        [HttpDelete]
        public ApiResult<bool> Delete(string id)
        {
            var trans = Transaction.GetById(_db, id);
            if (trans == null)
            {
                return new ApiResult<bool>
                {
                    Success = false,
                    Message = string.Format("Transaction {0} is not existing", id)
                };
            }

            return new ApiResult<bool>
            {
                Data = trans.Delete(_db, xISPUser.CurrentUserId)
            };
        }
    }
}
