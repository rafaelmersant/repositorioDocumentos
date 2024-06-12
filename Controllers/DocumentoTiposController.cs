using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;

namespace RepositorioDocumentos.Controllers
{
    public class DocumentoTiposController : Controller
    {
        // GET: DocumentoTipos
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                var db = new RepositorioDocRCEntities();

                var tipos = db.DocumentTypes.OrderBy(o => o.Description).ToList();
                return View(tipos);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        [HttpPost]
        public JsonResult AddDocumentType(string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var documentType = db.DocumentTypes.FirstOrDefault(o => o.Description.ToLower() == description.ToLower());
                    if (documentType != null) return Json(new { result = "500", message = "Este tipo de documento ya existe." });

                    db.DocumentTypes.Add(new DocumentType { Description = description, CreatedDate = DateTime.Now, CreatedBy = int.Parse(Session["userID"].ToString()) });
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
        public JsonResult UpdateDocumentType(int id, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var documentType = db.DocumentTypes.FirstOrDefault(o => o.Id == id);
                    if (documentType == null) return Json(new { result = "500", message = "Tipo de documento no encontrado." });
                    if (documentType.Description.ToLower() == description.ToLower() && documentType.Id != id) return Json(new { result = "500", message = "Este tipo de documento ya existe." });

                    documentType.Description = description;
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
        public JsonResult DeleteDocumentType(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var documentType = db.DocumentTypes.FirstOrDefault(o => o.Id == id);
                    if (documentType == null) return Json(new { result = "500", message = "Este tipo de documento no existe." });

                    db.DocumentTypes.Remove(documentType);
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

        public List<SelectListItem> GetDocumentoTipos()
        {
            List<SelectListItem> documentTypes = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                documentTypes.Add(new SelectListItem { Text = "Seleccionar Tipo de Doc.", Value = "" });
                var _documentTypes = db.DocumentTypes.ToArray();
                foreach (var item in _documentTypes)
                    documentTypes.Add(new SelectListItem { Text = item.Description, Value = item.Id.ToString() });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return documentTypes;
        }
    }
}