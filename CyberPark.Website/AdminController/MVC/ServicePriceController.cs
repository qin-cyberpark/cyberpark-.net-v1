using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CyberPark.Website.Controllers.MVC
{
    [Authorize(Roles = "Staff")]
    public class ServicePriceController : Controller
    {
        // GET: Service Price
        public ActionResult Index()
        {
            return View();
        }
    }
}