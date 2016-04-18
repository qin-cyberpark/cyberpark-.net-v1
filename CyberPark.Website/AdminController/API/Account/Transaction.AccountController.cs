using CyberPark.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CyberPark.Website.Controllers.API
{
    /// <summary>
    /// transaction part
    /// </summary>
    public partial class AccountController : ApiController
    {
        //GET: 
        [Route("api/account/{id}/transaction/recent")]
        public ApiResult<IList<Transaction>> GetRecentTransactionById(int id, [FromUri]int rows = 10)
        {
            var result = new ApiResult<IList<Transaction>>();
            result.Data = Transaction.GetRecentTransactionsByAccountId(_db, id, rows);
            return result;
        }
    }
}