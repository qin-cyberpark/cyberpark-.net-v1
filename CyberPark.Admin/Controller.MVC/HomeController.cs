using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CyberPark.Website.ViewModels;
using CyberPark.Domain.Core;

namespace CyberPark.Website.Controllers.MVC
{
    [Authorize(Roles = "Staff")]
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
            //get account counts
            ViewBag.AccountCounts = new HomeViewModels.AccountCountViewModel(Account.Get(_db,true));

            //get unmatched calls / services
            ViewBag.ExternalBillCounts = new HomeViewModels.ExternalBillCountViewModel(
                    ExternalBill.CountUnmatchedCall(_db),
                    ExternalBill.CountUnmatchedService(_db)
                );

            //get accountant count
            ViewBag.AccountantCounts = new HomeViewModels.AccountantCountViewModel(
                                    Invoice.GetUndeliveredInvoiceCount(_db));

            return View();
        }
    }
}