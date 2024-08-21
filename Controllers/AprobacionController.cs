using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class AprobacionController : Controller
    {
        [HttpPost]
        public JsonResult AddApproval(int documentHeaderId, int producedBy, string producedByName, int managerArea, 
                                        string managerAreaName, int directorArea, string directorAreaName)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var approval = db.DocumentApprovals.FirstOrDefault(o => o.DocumentHeaderId == documentHeaderId
                                                                && o.ProducedBy == producedBy
                                                                && o.ManagerArea == managerArea
                                                                && o.DirectorArea == directorArea);
                    if (approval != null) return Json(new { result = "500", message = "Este registro ya existe." });

                    db.DocumentApprovals.Add(new DocumentApproval
                    {
                        DocumentHeaderId = documentHeaderId,
                        ProducedBy = producedBy,
                        ProducedByName = producedByName,
                        ManagerArea = managerArea,
                        ManagerAreaName = managerAreaName,
                        DirectorArea = directorArea,
                        DirectorAreaName = directorAreaName,
                        CreatedDate = DateTime.Now,
                        CreatedBy = int.Parse(Session["userID"].ToString())
                    });
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"documentHeaderId: {documentHeaderId} | managerArea: {managerArea}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateApproval(int id, int documentHeaderId, int producedBy, string producedByName, int managerArea, 
                                            string managerAreaName, int directorArea, string directorAreaName)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var approval = db.DocumentApprovals.FirstOrDefault(o => o.Id == id);
                    if (approval == null) return Json(new { result = "500", message = "Registro no encontrado." });
                    if (approval.ProducedBy == producedBy
                        && approval.ManagerArea == managerArea
                        && approval.DirectorArea == directorArea
                        && approval.DocumentHeaderId == documentHeaderId
                        && approval.Id != id) return Json(new { result = "500", message = "Este registro ya existe." });

                    approval.ProducedBy = producedBy;
                    approval.ProducedByName = producedByName;
                    approval.ManagerArea = managerArea;
                    approval.ManagerAreaName = managerAreaName;
                    approval.DirectorArea = directorArea;
                    approval.DirectorAreaName = directorAreaName;
                    
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"Id: {id} | managerArea: {managerArea}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteApproval(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var approval = db.DocumentApprovals.FirstOrDefault(o => o.Id == id);
                    if (approval == null) return Json(new { result = "500", message = "Este registro no existe." });

                    db.DocumentApprovals.Remove(approval);
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
        public JsonResult GetApprovals(int documentHeaderId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var approvals = db.DocumentApprovals
                                       .Where(o => o.DocumentHeaderId == documentHeaderId)
                                       .Select(s => new
                                       {
                                           s.Id,
                                           s.DocumentHeaderId,
                                           s.ProducedBy,
                                           s.ProducedByName,
                                           s.ManagerArea,
                                           s.ManagerAreaName,
                                           s.DirectorArea,
                                           s.DirectorAreaName,
                                           s.CreatedDate,
                                           CreatedBy = s.User.Email
                                       })
                                       .OrderBy(o => o.CreatedDate)
                                       .AsEnumerable()
                                       .Select(s => new
                                       {
                                           s.Id,
                                           s.DocumentHeaderId,
                                           s.ProducedBy,
                                           s.ProducedByName,
                                           s.ManagerArea,
                                           s.ManagerAreaName,
                                           s.DirectorArea,
                                           s.DirectorAreaName,
                                           s.CreatedDate,
                                           s.CreatedBy
                                       })
                                       .ToArray();
                    return Json(new { result = "200", message = approvals });
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