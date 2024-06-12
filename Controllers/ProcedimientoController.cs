using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class ProcedimientoController : Controller
    {
        [HttpPost]
        public JsonResult AddProcedure(int documentHeaderId, short sortindex, string responsible, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var procedure = db.DocumentProcedures.FirstOrDefault(o => o.Description.ToLower() == description.ToLower()
                                                                        && o.Responsible.ToLower() == responsible.ToLower()
                                                                        && o.SortIndex == sortindex
                                                                        && o.DocumentHeaderId == documentHeaderId);
                    if (procedure != null) return Json(new { result = "500", message = "Este procedimiento ya existe." });

                    db.DocumentProcedures.Add(new DocumentProcedure
                    {
                        DocumentHeaderId = documentHeaderId,
                        SortIndex = sortindex,
                        Responsible = responsible,
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
        public JsonResult UpdateProcedure(int id, short sortindex, string responsible, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var procedure = db.DocumentProcedures.FirstOrDefault(o => o.Id == id);
                    if (procedure == null) return Json(new { result = "500", message = "Procedimiento no encontrado." });
                    if (procedure.Description.ToLower() == description.ToLower() 
                        && procedure.Responsible.ToLower() == responsible.ToLower()
                        && procedure.SortIndex == sortindex
                        && procedure.Id != id) return Json(new { result = "500", message = "Este procedimiento ya existe." });

                    procedure.SortIndex = sortindex;
                    procedure.Responsible = responsible;
                    procedure.Description = description;
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
        public JsonResult DeleteProcedure(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var procedure = db.DocumentProcedures.FirstOrDefault(o => o.Id == id);
                    if (procedure == null) return Json(new { result = "500", message = "Este procedimiento no existe." });

                    db.DocumentProcedures.Remove(procedure);
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
        public JsonResult GetProcedure(int documentHeaderId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var guidelines = db.DocumentProcedures
                                       .Where(o => o.DocumentHeaderId == documentHeaderId)
                                       .Select(s => new { s.Id, s.DocumentHeaderId, s.SortIndex, s.Responsible, s.Description, s.CreatedDate, CreatedBy = s.User.Email })
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