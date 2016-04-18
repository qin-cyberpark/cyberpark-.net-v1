using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;
using CyberPark.Website.Models;
namespace CyberPark.Website.Controllers.API
{
    /// <summary>
    /// account.invoice
    /// </summary>
    public partial class AccountController : ApiController
    {
        //GET: /api/account/id
        [Route("api/account/{accountId}/invoice/recent")]
        public ApiResult<IList<Invoice>> GetRecentInvoiceById(int accountId, [FromUri]int rows = 10)
        {
            var result = new ApiResult<IList<Invoice>>();
            result.Data = Invoice.GetRecentInvoicesByAccountId(_db, accountId, rows);
            return result;
        }

        [HttpDelete]
        [Route("api/account/invoice/{invoiceId}")]
        public ApiResult<bool> VoidInvoiceById(int invoiceId)
        {
            try {
                if (Invoice.Void(_db, invoiceId, xISPUser.CurrentUserId))
                {
                    return new ApiResult<bool> { Data = true };
                }
                else
                {
                    return new ApiResult<bool> { Data = false };
                }
            }
            catch(Exception ex)
            {
                return new ApiResult<bool> { Data = true, Message = ex.Message };
            }
        }

        [HttpGet]
        [Route("api/account/{accountId}/invoice/issue")]
        public ApiResult<bool> ReissueInvoice(int accountId, [FromUri]int invoiceId)
        {
            try
            {
                string msg = null;
                var acct = Account.Get(_db, accountId);
                if(acct == null)
                {
                    return new ApiResult<bool> { Data = false, Message = string.Format("Account {0} is not existing",accountId) };
                }

                var inv = acct.ReissueInvoice(_db, invoiceId, xISPUser.CurrentUserId, ref msg);
                if (inv != null)
                {
                    return new ApiResult<bool> { Data = true };
                }
                else
                {
                    return new ApiResult<bool> { Success = false, Message = msg};
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<bool> { Success = false, Message = ex.Message };
            }
        }
    }
}
