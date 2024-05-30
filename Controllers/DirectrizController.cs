using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class DirectrizController : Controller
    {
        [HttpPost]
        public JsonResult AddGuideline(int documentHeaderId, short sortindex, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var guideline = db.DocumentGuidelines.FirstOrDefault(o => o.Description.ToLower() == description.ToLower()
                                                                        && o.SortIndex == sortindex
                                                                        && o.DocumentHeaderId == documentHeaderId);
                    if (guideline != null) return Json(new { result = "500", message = "Esta directriz ya existe." });

                    db.DocumentGuidelines.Add(new DocumentGuideline
                    {
                        DocumentHeaderId = documentHeaderId,
                        SortIndex = sortindex,
                        Description = description,
                        CreatedDate = DateTime.Now,
                        CreatedBy = int.Parse(Session["userID"].ToString())
                    });
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
        public JsonResult UpdateGuideline(int id, short sortindex, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var guideline = db.DocumentGuidelines.FirstOrDefault(o => o.Id == id);
                    if (guideline == null) return Json(new { result = "500", message = "directriz no encontrada." });
                    if (guideline.Description.ToLower() == description.ToLower() && guideline.SortIndex == sortindex) return Json(new { result = "500", message = "Esta directriz ya existe." });

                    guideline.SortIndex = sortindex;
                    guideline.Description = description;
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
        public JsonResult DeleteGuideline(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var guideline = db.DocumentGuidelines.FirstOrDefault(o => o.Id == id);
                    if (guideline == null) return Json(new { result = "500", message = "Esta directriz no existe." });

                    db.DocumentGuidelines.Remove(guideline);
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
        public JsonResult GetGuideline(int documentHeaderId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var guidelines = db.DocumentGuidelines
                                       .Where(o => o.DocumentHeaderId == documentHeaderId)
                                       .Select(s => new { s.Id, s.DocumentHeaderId, s.SortIndex, s.Description, s.CreatedDate, CreatedBy = s.User.Email })
                                       .OrderBy(o => o.SortIndex)
                                       .ToArray();
                    return Json(new { result = "200", message = guidelines });
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"documentHeaderId: {documentHeaderId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }
    }
}