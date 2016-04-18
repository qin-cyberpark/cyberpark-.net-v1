using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CyberPark.Domain.Core;
namespace CyberPark.Website.Controllers.MVC
{
    public class StaffController : Controller
    {
        private xISPContext _db;
        public StaffController()
        {
            _db = new xISPContext();
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


        // GET: Staff
        public ActionResult Index()
        {
            ViewBag.Branches = Branch.Get(_db);
            return View();
        }
    }
}