using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Website.ViewModels;
using CyberPark.Domain.Core;

namespace CyberPark.Website.Controllers.API
{
    [Authorize(Roles = "Staff")]
    public class ProductController : ApiController
    {
        private xISPContext _db;
        public ProductController()
        {
            _db = new xISPContext();

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPut]
        public ApiResult<Product> Update(Product product)
        {
            return new ApiResult<Product>
            {
                Data = product.Save(_db)
            };
        }
    }
}
