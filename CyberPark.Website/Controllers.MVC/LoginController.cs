﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CyberPark.Website.Controllers.MVC
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.ReturnUrl = Request["ReturnUrl"];
            return View();
        }
    }
}