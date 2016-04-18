using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CyberPark.Website.ViewModels;
namespace CyberPark.Website.Controllers.MVC
{
    [Authorize(Roles = "Staff")]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {

            var cond = new AccountViewModels.AccountSearchCondition();
            cond.accountId = Request["accountId"]??"";
            cond.customerId = Request["customerId"] ?? "";
            cond.name = Request["name"] ?? "";
            cond.address = Request["address"] ?? "";
            cond.asid = Request["asid"] ?? "";
            cond.pstn = Request["pstn"] ?? "";
            cond.voip = Request["voip"] ?? "";
            cond.status = Request["status"] ?? "";
            ViewBag.SearchCondition = cond;
            return View();
        }
    }
}