using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;

namespace RepositorioDocumentos.Controllers
{
    public class DireccionController : Controller
    {
        // GET: Direccion
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                var db = new RepositorioDocRCEntities();

                var directorates = db.Directorates.OrderBy(o => o.Description).ToList();
                return View(directorates);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        [HttpPost]
        public JsonResult AddDirectorate(string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var directorate = db.Directorates.FirstOrDefault(o => o.Description.ToLower() == description.ToLower());
                    if (directorate != null) return Json(new { result = "500", message = "Esta dirección ya existe." });

                    db.Directorates.Add(new Directorate { Description = description, CreatedDate = DateTime.Now, CreatedBy = int.Parse(Session["userID"].ToString()) });
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
        public JsonResult UpdateDirectorate(int id, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var directorate = db.Directorates.FirstOrDefault(o => o.Id == id);
                    if (directorate == null) return Json(new { result = "500", message = "Dirección no encontrada." });
                    if (directorate.Description.ToLower() == description.ToLower()) return Json(new { result = "500", message = "Esta dirección ya existe." });

                    directorate.Description = description;
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
        public JsonResult DeleteDirectorate(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var directorate = db.Directorates.FirstOrDefault(o => o.Id == id);
                    if (directorate == null) return Json(new { result = "500", message = "Esta dirección no existe." });

                    db.Directorates.Remove(directorate);
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

        public List<SelectListItem> GetDirecciones()
        {
            List<SelectListItem> direcciones = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                direcciones.Add(new SelectListItem { Text = "Seleccionar Dirección", Value = "" });
                var _direcciones = db.Directorates.ToArray();
                foreach (var item in _direcciones)
                    direcciones.Add(new SelectListItem { Text = item.Description, Value = item.Id.ToString() });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return direcciones;
        }
    }
}