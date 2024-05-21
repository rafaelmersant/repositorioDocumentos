using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;

namespace RepositorioDocumentos.Controllers
{
    public class MacroprocesoController : Controller
    {
        // GET: Macroproceso
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                var db = new RepositorioDocRCEntities();

                var macroprocesos = db.Macroprocesses.OrderBy(o => o.Description).ToList();
                return View(macroprocesos);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        [HttpPost]
        public JsonResult AddMacroprocess(string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var macroprocess = db.Macroprocesses.FirstOrDefault(o => o.Description.ToLower() == description.ToLower());
                    if (macroprocess != null) return Json(new { result = "500", message = "Este macroproceso ya existe." });

                    db.Macroprocesses.Add(new Macroprocess { Description = description, CreatedDate = DateTime.Now, CreatedBy = int.Parse(Session["userID"].ToString()) });
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"description: {description}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateMacroprocess(int id, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var macroprocess = db.Macroprocesses.FirstOrDefault(o => o.Id == id);
                    if (macroprocess == null) return Json(new { result = "500", message = "Macroproceso no encontrado." });
                    if (macroprocess.Description.ToLower() == description.ToLower()) return Json(new { result = "500", message = "Este macroproceso ya existe." });

                    macroprocess.Description = description;
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"Id: {id} | description: {description}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteMacroprocess(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var macroprocess = db.Macroprocesses.FirstOrDefault(o => o.Id == id);
                    if (macroprocess == null) return Json(new { result = "500", message = "Este macroproceso no existe." });

                    db.Macroprocesses.Remove(macroprocess);
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"id: {id}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        public List<SelectListItem> GetMacroprocesos()
        {
            List<SelectListItem> macroprocesos = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                macroprocesos.Add(new SelectListItem { Text = "Seleccionar Macroproceso", Value = "" });
                var _macroprocesos = db.Macroprocesses.ToArray();
                foreach (var item in _macroprocesos)
                    macroprocesos.Add(new SelectListItem { Text = item.Description, Value = item.Id.ToString() });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return macroprocesos;
        }
    }
}