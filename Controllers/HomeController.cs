﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Login", "User");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}