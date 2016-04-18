using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CyberPark.Domain.Core;
using CyberPark.Website.Models;
using CyberPark.Website.ViewModels;
namespace CyberPark.Website.Controllers.MVC
{
    public class ApplicationController : Controller
    {
        private xISPContext _db;
        public ApplicationController()
        {
            _db = new xISPContext();
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        // GET: Application
        [HttpPost]
        public ActionResult Index()
        {
            var model = new ApplicationViewModels.Summary();
            model.Account = new Account
            {
                Address = Request["address"],
                FirstName = xISPUser.CurrentUser?.Customer?.FirstName,
                LastName = xISPUser.CurrentUser?.Customer?.LastName,
                Email = xISPUser.CurrentUser?.Customer?.Email,
                Mobile = xISPUser.CurrentUser?.Customer?.Mobile,
                Type = bool.Parse(Request["isBusiness"]) ? Account.Types.Business : Account.Types.Personal
            };

            model.Plan = new Plan
            {
                Id = Request["planId"]
            };

            var broadbandType = Request["broadbandType"];
            if (string.IsNullOrEmpty(model.Account.Address) || string.IsNullOrEmpty(model.Plan.Id))
            {
                return RedirectToAction("Availability", "Application");
            }

            ViewBag.Plans = Plan.Get(_db, model.Account.Type.Equals(Account.Types.Business), broadbandType);
            return View(model);
        }

        //
        public ActionResult Personal()
        {
            ViewBag.Address = Request["address"];
            ViewBag.IsBusiness = false;
            return View("Availability");
        }

        public ActionResult Business()
        {
            ViewBag.Address = Request["address"];
            ViewBag.IsBusiness = true;
            return View("Availability");
        }

        [HttpPost]
        public ActionResult Submit(ApplicationViewModels.Summary model)
        {
            Session["APPLICATION"] = model;
            return Redirect("/application/preview");
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public ActionResult Preview()
        {
            var model = (ApplicationViewModels.Summary)Session["APPLICATION"];
            if (model == null)
            {
                return Redirect("/");
            }
            model.Plan = Plan.Get(_db, model.Plan.Id);
            if (string.IsNullOrEmpty(model.Account.Name))
            {
                model.Account.FirstName = xISPUser.CurrentUser?.Customer?.FirstName;
                model.Account.LastName = xISPUser.CurrentUser?.Customer?.LastName;
                model.Account.Email = xISPUser.CurrentUser?.Email;
                model.Account.Mobile = xISPUser.CurrentUser?.PhoneNumber;
            }
            return View(model);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public ActionResult Checkout()
        {
            var model = (ApplicationViewModels.Summary)Session["APPLICATION"];
            if (model == null)
            {
                return Redirect("/");
            }

            //account info
            model.Account.CustomerId = xISPUser.CurrentUser.Customer.Id;
            model.Account.RegisterBranchId = SysConfig.Instance.DefaultBranchId;
            model.Account.ChargeBranchId = SysConfig.Instance.DefaultBranchId;
            //

            var account = Account.CreateApplication(_db, model.Account, model.Plan, model.PayMonthly ? 1 : 12);

            return Redirect("/application/prepay/" + account.Products.FirstOrDefault().Id);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult prepay(string id)
        {
            var product = Product.Get(_db, id);
            return View(product);
        }
    }
}