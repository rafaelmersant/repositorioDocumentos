using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using static RepositorioDocumentos.ViewModels.DocumentPreviewViewModel;

namespace RepositorioDocumentos.Controllers
{
    public class DocumentoController : Controller
    {
        // GET: Documento
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                ViewBag.DocumentoTipos = new DocumentoTiposController().GetDocumentoTipos();
                ViewBag.DocumentoCodigos = new DocumentoCodigoController().GetDocumentoCodigos();
                ViewBag.Direcciones = new DireccionController().GetDirecciones();

                ViewBag.Areas = new List<SelectListItem>();
                ViewBag.Departamentos = new List<SelectListItem>();
                ViewBag.Procesos = new List<SelectListItem>();

                ViewBag.Macroprocesos = new MacroprocesoController().GetMacroprocesos();

                var db = new RepositorioDocRCEntities();

                var documents = db.DocumentHeaders.OrderByDescending(o => o.Id).ToList();
                return View(documents);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        public ActionResult Documento()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                ViewBag.DocumentoTipos = new DocumentoTiposController().GetDocumentoTipos();
                ViewBag.DocumentoCodigos = new DocumentoCodigoController().GetDocumentoCodigos();
                ViewBag.Direcciones = new DireccionController().GetDirecciones();

                ViewBag.Areas = new List<SelectListItem>();
                ViewBag.Departamentos = new List<SelectListItem>();
                ViewBag.Procesos = new List<SelectListItem>();

                ViewBag.Macroprocesos = new MacroprocesoController().GetMacroprocesos();
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }

            return View();
        }

        public ActionResult VistaPreliminar(int? id)
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            if (id <= 0 || id == null) return RedirectToAction("Index", "Documento");

            DocumentContainer documentContainer = new DocumentContainer();

            try
            {
                var db = new RepositorioDocRCEntities();
               
                var documentHeader = db.DocumentHeaders.FirstOrDefault(h => h.Id == id);
                var documentDetail = db.DocumentDetails.FirstOrDefault(d => d.DocumentHeaderId == id);
                var documentContent = db.DocumentContents.FirstOrDefault(d => d.DocumentHeaderId == id);

                var glossary = db.DocumentGlossaries.Where(g => g.DocumentHeaderId == id).ToList();
                var guidelines = db.DocumentGuidelines.Where(g => g.DocumentHeaderId == id).OrderBy(o => o.SortIndex).ToList();
                var procedures = db.DocumentProcedures.Where(p => p.DocumentHeaderId == id).OrderBy(o => o.SortIndex).ToList();
                var references = db.DocumentReferences.Where(r => r.DocumentHeaderId == id).ToList();
                var changes = db.DocumentChanges.Where(c => c.DocumentHeaderId == id).ToList();

                documentContainer.DocumentHeader = documentHeader;
                documentContainer.DocumentDetail = documentDetail;
                documentContainer.DocumentContent = documentContent;

                documentContainer.DocumentGlossary = glossary;
                documentContainer.DocumentGuidelines = guidelines;
                documentContainer.DocumentProcedures = procedures;
                documentContainer.DocumentReferences = references;
                documentContainer.DocumentChanges = changes;
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }

            return View(documentContainer);
        }

        [HttpPost]
        public JsonResult SaveDocumentHeader(DocumentHeader documentHeader)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                DocumentHeader _documentHeader = new DocumentHeader();

                using (var db = new RepositorioDocRCEntities())
                {
                    var docHeader = db.DocumentHeaders.FirstOrDefault(d => d.Title.ToLower() == documentHeader.Title.ToLower()
                                                                       && d.DirectorateId == documentHeader.DirectorateId
                                                                       && d.AreaId == documentHeader.AreaId
                                                                       && d.DepartmentCode == documentHeader.DepartmentCode
                                                                       && d.MacroprocessId == documentHeader.MacroprocessId
                                                                       && d.ProcessId == documentHeader.ProcessId);

                    if (docHeader != null && documentHeader.Id == 0) return Json(new { result = "500", message = "Este documento ya existe." });

                    if (docHeader != null)
                    {
                        docHeader.DocumentTypeId = documentHeader.DocumentTypeId;
                        docHeader.Status = documentHeader.Status;
                        docHeader.Image = documentHeader.Image;
                        docHeader.Revision = documentHeader.Revision;
                        docHeader.Date = documentHeader.Date;
                        docHeader.Title = documentHeader.Title;
                        docHeader.DirectorateId = documentHeader.DirectorateId;
                        docHeader.AreaId = documentHeader.AreaId;
                        docHeader.DepartmentCode = documentHeader.DepartmentCode;
                        docHeader.MacroprocessId = documentHeader.MacroprocessId;
                        docHeader.ProcessId = documentHeader.ProcessId;
                        docHeader.Objective = documentHeader.Objective;
                        
                        db.Entry(docHeader).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        _documentHeader = docHeader;
                    }
                    else
                    {
                        _documentHeader = db.DocumentHeaders.Add(new DocumentHeader
                        {
                            DocumentTypeId = documentHeader.DocumentTypeId,
                            Status = documentHeader.Status,
                            Image = documentHeader.Image,
                            Code = documentHeader.Code,
                            Revision = documentHeader.Revision,
                            Date = documentHeader.Date,
                            Title = documentHeader.Title,
                            DirectorateId = documentHeader.DirectorateId,
                            AreaId = documentHeader.AreaId,
                            DepartmentCode = documentHeader.DepartmentCode,
                            MacroprocessId = documentHeader.MacroprocessId,
                            ProcessId = documentHeader.ProcessId,
                            Objective = documentHeader.Objective,
                            CreatedDate = DateTime.Now,
                            CreatedBy = int.Parse(Session["userID"].ToString())
                        });
                        db.SaveChanges();
                    }
                }

                return Json(new { result = "200", message = _documentHeader.Id });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"title: {documentHeader.Title}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveDocumentContent(DocumentContent documentContent)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                DocumentContent _documentContent = new DocumentContent();

                documentContent.Body = string.IsNullOrEmpty(documentContent.Body) ? "" : documentContent.Body;
                using (var db = new RepositorioDocRCEntities())
                {
                    _documentContent = db.DocumentContents.FirstOrDefault(d => d.DocumentHeaderId == documentContent.DocumentHeaderId);

                    if (_documentContent != null)
                    {
                        _documentContent.Body = documentContent.Body ;
                        db.Entry(_documentContent).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        _documentContent = db.DocumentContents.Add(new DocumentContent
                        {
                            DocumentHeaderId = documentContent.DocumentHeaderId,
                            Body = documentContent.Body,
                            CreatedDate = DateTime.Now,
                            CreatedBy = int.Parse(Session["userID"].ToString())
                        });
                    }

                    db.SaveChanges();
                }

                return Json(new { result = "200", message = _documentContent });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"documentHeaderId: {documentContent.DocumentHeaderId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveDocumentDetail(DocumentDetail documentDetail)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                DocumentDetail _documentDetail = new DocumentDetail();

                using (var db = new RepositorioDocRCEntities())
                {
                    _documentDetail = db.DocumentDetails.FirstOrDefault(d => d.DocumentHeaderId == documentDetail.DocumentHeaderId);

                    if (_documentDetail != null)
                    {
                        _documentDetail.Scope = documentDetail.Scope;
                        _documentDetail.Responsabilities = documentDetail.Responsabilities;
                        _documentDetail.Policy = documentDetail.Policy;
                        db.Entry(_documentDetail).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        _documentDetail = db.DocumentDetails.Add(new DocumentDetail
                        {
                            DocumentHeaderId = documentDetail.DocumentHeaderId,
                            Scope = documentDetail.Scope,
                            Responsabilities = documentDetail.Responsabilities,
                            Policy = documentDetail.Policy,
                            CreatedDate = DateTime.Now,
                            CreatedBy = int.Parse(Session["userID"].ToString())
                        });
                    }

                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"documentHeaderId: {documentDetail.DocumentHeaderId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetDocument(int documentHeaderId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var documentHeader = db.DocumentHeaders
                        .Where(d => d.Id == documentHeaderId)
                        .Select(s => new
                        {
                            s.Id,
                            s.DocumentTypeId,
                            s.Status,
                            s.Image,
                            s.Code,
                            s.Revision,
                            s.Date,
                            s.Title,
                            s.DirectorateId,
                            s.DepartmentCode,
                            s.AreaId,
                            s.MacroprocessId,
                            s.ProcessId,
                            s.Objective,
                            s.CreatedDate,
                            CreatedBy = s.User.Email
                        })
                        .AsEnumerable()
                        .Select(s => new
                        {
                            s.Id,
                            s.DocumentTypeId,
                            s.Status,
                            s.Image,
                            s.Code,
                            s.Revision,
                            Date = s.Date.ToString("dd/MM/yyyy"), // Format date in memory
                            s.Title,
                            s.DirectorateId,
                            s.DepartmentCode,
                            s.AreaId,
                            s.MacroprocessId,
                            s.ProcessId,
                            s.Objective,
                            s.CreatedDate,
                            s.CreatedBy
                        })
                        .FirstOrDefault();

                    var documentDetail = db.DocumentDetails
                        .Where(d => d.DocumentHeaderId == documentHeaderId)
                        .Select(s => new
                        {
                            s.Id,
                            s.Scope,
                            s.Responsabilities,
                            s.Policy,
                            s.CreatedDate,
                            CreatedBy = s.User.Email
                        })
                        .AsEnumerable()
                        .Select(s => new
                        {
                            s.Scope,
                            s.Responsabilities,
                            s.Policy,
                            CreatedDate = s.CreatedDate.ToString("dd/MM/yyyy"),
                            s.CreatedBy
                        })
                        .FirstOrDefault();

                    var documentContent = db.DocumentContents.Where(d => d.DocumentHeaderId == documentHeaderId).Select(s => new { s.Id, s.Body }).FirstOrDefault();

                    return Json(new { result = "200", message = new { documentHeader, documentDetail, documentContent }});
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