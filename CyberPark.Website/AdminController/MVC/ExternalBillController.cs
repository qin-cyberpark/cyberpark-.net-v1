using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CyberPark.Domain.Core;
using System.Configuration;
using CyberPark.Website.Models;
namespace CyberPark.Website.Controllers.MVC
{
    [Authorize(Roles = "Staff")]
    public class ExternalBillController : Controller
    {
        // GET: ExternalBill
        public ActionResult Index()
        {
            ViewBag.ExternalBills = ExternalBill.Get().OrderByDescending(x => x.OperatedDate).ToArray();
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(SysConfig.Instance.TemporaryDirectory + file.FileName);
                    ExternalBill bill = ExternalBill.Parse(SysConfig.Instance.TemporaryDirectory, file.FileName);
                    if (bill != null)
                    {
                        Session["TMP_OUTER_BILL_CSV"] = bill;
                        ViewBag.ExternalBills = new List<ExternalBill>();
                        ViewBag.ExternalBill = bill;
                        ViewBag.IsShowInsertConform = true;
                        return View("Index");
                    }
                }
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult Insert()
        {
            ExternalBill bill = (ExternalBill)Session["TMP_OUTER_BILL_CSV"];
            string oriPath = SysConfig.Instance.TemporaryDirectory + bill.FileName;
            string filePath = SysConfig.Instance.ExternalBillDirectory + bill.FileName;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            System.IO.File.Move(oriPath, filePath);
            try {
                bill.Save(xISPUser.CurrentUserId);
            }catch{

            }
            return RedirectToAction("Index");
        }

        public ActionResult Detail(string id)
        {
            ViewBag.ExternalBill = ExternalBill.Get(id);
            return View();
        }

    }
}