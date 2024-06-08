using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class NotificacionCambiosController : Controller
    {
        [HttpPost]
        public JsonResult AddChange(int documentHeaderId, DateTime changeDate, short revision, short pagesAffected, string originator, string natureChange)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var change = db.DocumentChanges.FirstOrDefault(o => o.DocumentHeaderId == documentHeaderId
                                                                && o.Revision == revision
                                                                && o.PagesAffected == pagesAffected
                                                                && o.Originator.ToLower() == originator.ToLower()
                                                                && o.NatureChange.ToLower() == natureChange.ToLower());
                    if (change != null) return Json(new { result = "500", message = "Este cambio ya existe." });

                    db.DocumentChanges.Add(new DocumentChange
                    {
                        DocumentHeaderId = documentHeaderId,
                        Date = changeDate,
                        Revision = revision,
                        PagesAffected = pagesAffected,
                        Originator = originator,
                        NatureChange = natureChange,
                        CreatedDate = DateTime.Now,
                        CreatedBy = int.Parse(Session["userID"].ToString())
                    });
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"documentHeaderId: {documentHeaderId} | Originador: {originator}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateChange(int id, DateTime changeDate, short revision, short pagesAffected, string originator, string natureChange)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var change = db.DocumentChanges.FirstOrDefault(o => o.Id == id);
                    if (change == null) return Json(new { result = "500", message = "Cambio no encontrado." });
                    if (change.Originator.ToLower() == originator.ToLower()
                        && change.NatureChange.ToLower() == natureChange.ToLower()
                        && change.Revision == revision) return Json(new { result = "500", message = "Este cambio ya existe." });

                    change.Date = changeDate;
                    change.Revision = revision;
                    change.PagesAffected = pagesAffected;
                    change.Originator = originator;
                    change.NatureChange = natureChange;
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"Id: {id} | Originador: {originator}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteChange(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var change = db.DocumentChanges.FirstOrDefault(o => o.Id == id);
                    if (change == null) return Json(new { result = "500", message = "Este cambio no existe." });

                    db.DocumentChanges.Remove(change);
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
        public JsonResult GetChanges(int documentHeaderId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var guidelines = db.DocumentChanges
                                       .Where(o => o.DocumentHeaderId == documentHeaderId)
                                       .Select(s => new
                                       {
                                           s.Id,
                                           s.DocumentHeaderId,
                                           s.Date,
                                           s.Revision,
                                           s.PagesAffected,
                                           s.Originator,
                                           s.NatureChange,
                                           s.CreatedDate,
                                           CreatedBy = s.User.Email
                                       })
                                       .OrderBy(o => o.Date)
                                       .AsEnumerable()
                                       .Select(s => new
                                       {
                                           s.Id,
                                           s.DocumentHeaderId,
                                           Date = s.Date.ToString("dd/MM/yyyy"),
                                           s.Revision,
                                           s.PagesAffected,
                                           s.Originator,
                                           s.NatureChange,
                                           s.CreatedDate,
                                           s.CreatedBy
                                       })
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