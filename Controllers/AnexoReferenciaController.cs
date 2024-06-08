using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class AnexoReferenciaController : Controller
    {
        [HttpPost]
        public JsonResult UploadFile()
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                HttpPostedFileBase file = Request.Files["file"];

                if (file != null && file.ContentLength > 0)
                {
                    string description = Request.Form["description"];
                    int documentHeaderId = int.Parse(Request.Form["DocumentHeaderId"]);
                    int userId = int.Parse(Session["userID"].ToString());

                    string fileName = Path.GetFileName(file.FileName);
                    string newFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{documentHeaderId}{Path.GetExtension(fileName)}";
                    string filePath = Path.Combine(Server.MapPath("~/AnexosReferencias"), newFileName);

                    file.SaveAs(filePath);

                    try
                    {
                        using (var db = new RepositorioDocRCEntities())
                        {
                            db.DocumentReferences.Add(new DocumentReference
                            {
                                DocumentHeaderId = documentHeaderId,
                                Name = description,
                                Url = newFileName,
                                CreatedDate = DateTime.Now,
                                CreatedBy = userId
                            });
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Helper.SendException(ex);
                    }

                    return Json(new { result = "200", message = "success" });
                }
                else
                {
                    return Json(new { result = "404", message = "No ha seleccionado" });
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetUploadedFiles(int documentHeaderId)
        {
            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    var references = db.DocumentReferences
                                       .Where(d => d.DocumentHeaderId == documentHeaderId)
                                       .Select(s => new {s.Id, s.Url, s.Name})
                                       .ToList();
                    return Json(new { result = "200", message = references });
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"documentHeaderId: {documentHeaderId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteFile(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var reference = db.DocumentReferences.FirstOrDefault(d => d.Id == id);
                    if (reference != null)
                    {
                        db.DocumentReferences.Remove(reference);
                        db.SaveChanges();

                        string filePathToDelete = Server.MapPath($"~/AnexosReferencias/{reference.Url}");
                        if (System.IO.File.Exists(filePathToDelete))
                        {
                            System.IO.File.Delete(filePathToDelete);
                            return Json(new { result = "200", message = "success" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"fileId: {id}");

                return Json(new { result = "500", message = ex.Message });
            }

            return Json(new { result = "404", message = "El archivo no fue encontrado." });
        }
    }
}