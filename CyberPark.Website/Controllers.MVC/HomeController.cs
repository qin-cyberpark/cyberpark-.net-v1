using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CyberPark.Domain.Core;
using CyberPark.Website.ViewModels;
using CyberPark.Website.Models;
namespace CyberPark.Website.Controllers
{
    public class HomeController : Controller
    {
        private xISPContext _db;
        public HomeController()
        {
            _db = new xISPContext();
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        // GET: Login
        public ActionResult Login()
        {
            if (xISPUser.HasCustomerLogin)
            {
                return Redirect("/");
            }
            ViewBag.ReturnUrl = Request["ReturnUrl"];
            return View();
        }

        // GET: Logout
        [HttpPost]
        public ActionResult Logout()
        {
            ViewBag.ReturnUrl = Request["ReturnUrl"];
            return Redirect("/");
        }

        //Sign Up
        public ActionResult SignUp()
        {
            ViewBag.ReturnUrl = Request["ReturnUrl"];
            return View();
        }

        //customer
        [Authorize(Roles="Customer")]
        public ActionResult Customer()
        {
            var customer = Domain.Core.Customer.GetById(_db, xISPUser.CurrentUserId);
            return View(customer);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult MyAddress(int id)
        {
            var account = Account.Get(_db, id, true);
            account.Customer = Domain.Core.Customer.GetById(_db, account.CustomerId);
            return View(account);
        }
    }
}