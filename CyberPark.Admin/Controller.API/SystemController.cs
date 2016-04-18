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
    [Authorize(Roles = "Staff")]
    public class SystemController : ApiController
    {
        private xISPContext _db;
        public SystemController()
        {
            _db = new xISPContext();
        }

        // GET api/<controller>/5
        [Route("api/sys/warning")]
        public ApiResult<IList<Warning>> GetWarning()
        {
            var result = new ApiResult<IList<Warning>>();
            result.Data = Warning.Get(_db);
            return result;
        }

        [Route("api/sys/warning/{id}")]
        [HttpDelete]
        public ApiResult<bool> ClearWarning(string id)
        {
            var result = new ApiResult<bool>();
            result.Data = Warning.Clear(_db, id, xISPUser.CurrentUserId);
            return result;
        }
    }
}