using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CyberPark.Domain.Core;
namespace CyberPark.Website.Controllers.MVC
{
    [Authorize(Roles = "Staff")]
    public class InvoiceController : Controller
    {
        private xISPContext _db;
        public InvoiceController()
        {
            _db = new xISPContext();
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


        // GET: Invoice
        public ActionResult View(int id)
        {
            var inv = Invoice.GetById(_db, id);
            //if (inv.IsVoid)
            //{
            //    inv = Utility.ToObject<Invoice>(inv.JsonData);
            //}
            ViewBag.Invoice = inv;

            return View();
        }
    }
}