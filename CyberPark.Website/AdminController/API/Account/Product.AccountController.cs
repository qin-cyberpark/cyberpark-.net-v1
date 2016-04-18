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
        //GET:
        [Route("api/account/{id}/Product/")]
        public ApiResult<IList<Product>> Get(int id, [FromUri] string repeatTypes)
        {
            var result = new ApiResult<IList<Product>>();
            switch (repeatTypes.ToLower())
            {
                case "all":
                    result.Data = Product.GetProductsByAccountId(_db, id);
                    break;
                case "monthly":
                    result.Data = Product.GetMonthlyProductsByAccountId(_db, id);
                    break;
                case "one-off":
                    result.Data = Product.GetOneOffProductsByAccountId(_db, id);
                    break;
            }

            return result;
        }
    }
}
