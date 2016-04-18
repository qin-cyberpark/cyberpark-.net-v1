using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CyberPark.Domain.Core;
namespace CyberPark.Website.Controllers.MVC
{
    [Authorize(Roles = "Staff")]
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        // GET: Customer/id
        public ActionResult View(int id)
        {
            ViewBag.CustomerId = id;
            return View();
        }
    }
}