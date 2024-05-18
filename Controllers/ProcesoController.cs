using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;

namespace RepositorioDocumentos.Controllers
{
    public class ProcesoController : Controller
    {
        // GET: Proceso
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                ViewBag.Macroprocesos = new MacroprocesoController().GetMacroprocesos();

                var db = new RepositorioDocRCEntities();

                var procesos = db.Processes.OrderBy(o => o.Description).ToList();
                return View(procesos);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        [HttpPost]
        public JsonResult AddProcess(int macroprocessId, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var process = db.Processes.FirstOrDefault(o => o.Description.ToLower() == description.ToLower() && o.MacroprocessId == macroprocessId);
                    if (process != null) return Json(new { result = "500", message = "Este proceso ya existe." });

                    db.Processes.Add(new Process { MacroprocessId = macroprocessId, Description = description, CreatedDate = DateTime.Now, CreatedBy = int.Parse(Session["userID"].ToString()) });
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
        public JsonResult UpdateProcess(int id, int macroprocessId, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var process = db.Processes.FirstOrDefault(o => o.Id == id);
                    if (process == null) return Json(new { result = "500", message = "Proceso no encontrado." });
                    if (process.Description.ToLower() == description.ToLower() && process.MacroprocessId == macroprocessId) return Json(new { result = "500", message = "Este proceso ya existe." });

                    process.MacroprocessId = macroprocessId;
                    process.Description = description;
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
        public JsonResult DeleteProcess(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var process = db.Processes.FirstOrDefault(o => o.Id == id);
                    if (process == null) return Json(new { result = "500", message = "Este proceso no existe." });

                    db.Processes.Remove(process);
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

        public List<SelectListItem> Processos()
        {
            List<SelectListItem> procesos = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                procesos.Add(new SelectListItem { Text = "Seleccionar", Value = "" });
                var _procesos = db.Processes.ToArray();
                foreach (var item in _procesos)
                    procesos.Add(new SelectListItem { Text = item.Description, Value = item.Id.ToString() });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return procesos;
        }
    }
}