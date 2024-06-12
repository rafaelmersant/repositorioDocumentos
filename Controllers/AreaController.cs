using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;

namespace RepositorioDocumentos.Controllers
{
    public class AreaController : Controller
    {
        // GET: Area
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                var db = new RepositorioDocRCEntities();
                
                ViewBag.Direcciones = new DireccionController().GetDirecciones();

                var areas = db.Areas.OrderBy(o => o.Description).ToList();
                return View(areas);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        [HttpPost]
        public JsonResult AddArea(short directorateId, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var area = db.Areas.FirstOrDefault(o => o.Description.ToLower() == description.ToLower() && o.DirectorateId == directorateId);
                    if (area != null) return Json(new { result = "500", message = "Esta área ya existe." });

                    db.Areas.Add(new Area {DirectorateId = directorateId, Description = description, CreatedDate = DateTime.Now, CreatedBy = int.Parse(Session["userID"].ToString()) });
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
        public JsonResult UpdateArea(int id, short directorateId, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var area = db.Areas.FirstOrDefault(o => o.Id == id);
                    if (area == null) return Json(new { result = "500", message = "Area no encontrada." });
                    if (area.Description.ToLower() == description.ToLower() 
                        && area.DirectorateId == directorateId
                        && area.Id != id) return Json(new { result = "500", message = "Esta área ya existe." });

                    area.DirectorateId = directorateId;
                    area.Description = description;
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
        public JsonResult DeleteArea(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var area = db.Areas.FirstOrDefault(o => o.Id == id);
                    if (area == null) return Json(new { result = "500", message = "Esta área no existe." });

                    db.Areas.Remove(area);
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

        [HttpPost]
        public JsonResult GetAreasByDirectorateId(int directorateId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                var areas = GetAreas(directorateId);

                return Json(new { result = "200", message = areas });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"directorateId: {directorateId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        public List<SelectListItem> GetAreas(int directorateId = 0)
        {
            List<SelectListItem> areas = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                areas.Add(new SelectListItem { Text = "Seleccionar Area", Value = "" });
                var _areas = db.Areas.ToArray();
                if (directorateId > 0) _areas = _areas.Where(a => a.DirectorateId == directorateId).ToArray();

                foreach (var item in _areas)
                    areas.Add(new SelectListItem { Text = item.Description, Value = item.Id.ToString() });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return areas;
        }
    }
}