using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class GlosarioController : Controller
    {
        [HttpPost]
        public JsonResult AddGlossary(int documentHeaderId, string word, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var glossary = db.DocumentGlossaries.FirstOrDefault(o => o.Description.ToLower() == description.ToLower()
                                                                        && o.Word.ToLower() == word.ToLower()
                                                                        && o.DocumentHeaderId == documentHeaderId);
                    if (glossary != null) return Json(new { result = "500", message = "Esta palabra ya existe." });

                    db.DocumentGlossaries.Add(new DocumentGlossary
                    {
                        DocumentHeaderId = documentHeaderId,
                        Word = word,
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
        public JsonResult UpdateGlossary(int id, string word, string description)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var glossary = db.DocumentGlossaries.FirstOrDefault(o => o.Id == id);
                    if (glossary == null) return Json(new { result = "500", message = "Palabra no encontrada." });
                    if (glossary.Description.ToLower() == description.ToLower() 
                        && glossary.Word.ToLower() == word.ToLower()
                        && glossary.Id != id) return Json(new { result = "500", message = "Esta palabra ya existe." });

                    glossary.Word = word;
                    glossary.Description = description;
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
        public JsonResult DeleteGlossary(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var glossary = db.DocumentGlossaries.FirstOrDefault(o => o.Id == id);
                    if (glossary == null) return Json(new { result = "500", message = "Esta palabra no existe." });

                    db.DocumentGlossaries.Remove(glossary);
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
        public JsonResult GetGlossary(int documentHeaderId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var glossaries = db.DocumentGlossaries
                                       .Where(o => o.DocumentHeaderId == documentHeaderId)
                                       .Select(s => new { s.Id, s.DocumentHeaderId, s.Word, s.Description, s.CreatedDate, CreatedBy = s.User.Email })
                                       .ToArray();
                    return Json(new { result = "200", message = glossaries });
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