using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;

namespace RepositorioDocumentos.Controllers
{
    public class DocumentoCodigoController : Controller
    {
        // GET: DocumentoCodigo
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                ViewBag.Directorates = new DireccionController().GetDirecciones();
                ViewBag.Areas = new AreaController().GetAreas();
                ViewBag.Departments = new DepartamentoController().GetDepartamentos();

                var db = new RepositorioDocRCEntities();

                var tipos = db.DocumentCodes.OrderBy(o => o.Code).ToList();
                return View(tipos);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        [HttpPost]
        public JsonResult AddDocumentCode(string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var documentCode = db.DocumentCodes.FirstOrDefault(o => o.Code.ToLower() == description.ToLower());
                    if (documentCode != null) return Json(new { result = "500", message = "Este código ya existe." });

                    db.DocumentCodes.Add(new DocumentCode { Code = description, CreatedDate = DateTime.Now, CreatedBy = int.Parse(Session["userID"].ToString()) });
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
        public JsonResult UpdateDocumentCode(int id, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var documentCode = db.DocumentCodes.FirstOrDefault(o => o.Id == id);
                    if (documentCode == null) return Json(new { result = "500", message = "Código no encontrado." });
                    if (documentCode.Code.ToLower() == description.ToLower() && documentCode.Id != id) return Json(new { result = "500", message = "Este código ya existe." });

                    documentCode.Code = description;
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
        public JsonResult DeleteDocumentCode(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var documentCode = db.DocumentCodes.FirstOrDefault(o => o.Id == id);
                    if (documentCode == null) return Json(new { result = "500", message = "Este código no existe." });

                    db.DocumentCodes.Remove(documentCode);
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

        public List<SelectListItem> GetDocumentoCodigos()
        {
            List<SelectListItem> documentoCodigos = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                documentoCodigos.Add(new SelectListItem { Text = "Seleccionar Código", Value = "" });
                var _documentoCodigos = db.DocumentCodes.ToArray();
                foreach (var item in _documentoCodigos)
                    documentoCodigos.Add(new SelectListItem { Text = item.Code, Value = item.Id.ToString() });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return documentoCodigos;
        }
    }
}