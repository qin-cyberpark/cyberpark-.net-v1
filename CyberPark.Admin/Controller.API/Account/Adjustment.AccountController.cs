using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;

namespace CyberPark.Website.Controllers.API
{
    public partial class AccountController : ApiController
    {
        //GET: /api/account/id
        [Route("api/account/{id}/adjustment/recent")]
        public ApiResult<IList<Adjustment>> GetRecentAdjustmentById(int id, [FromUri]int rows = 10)
        {
            var result = new ApiResult<IList<Adjustment>>();
            result.Data = Adjustment.GetRecentAdjustmentsByAccountId(_db, id, rows);
            return result;
        }
    }
}
