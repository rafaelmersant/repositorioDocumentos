using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;

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

                //ViewBag.Areas = new AreaController().GetAreas();
                //ViewBag.Departamentos = new DepartamentoController().GetDepartamentos();
                //ViewBag.Procesos = new ProcesoController().GetProcessos();

                ViewBag.Macroprocesos = new MacroprocesoController().GetMacroprocesos();

                var db = new RepositorioDocRCEntities();

                var documents = db.DocumentHeaders.OrderBy(o => o.Id).ToList();
                return View(documents);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        public ActionResult Nuevo()
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

                //ViewBag.Areas = new AreaController().GetAreas();
                //ViewBag.Departamentos = new DepartamentoController().GetDepartamentos();
                //ViewBag.Procesos = new ProcesoController().GetProcessos();

                ViewBag.Macroprocesos = new MacroprocesoController().GetMacroprocesos();
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }

            return View();
        }

        [HttpPost]
        public JsonResult AddDocumentHeader(DocumentHeader documentHeader)
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

                    if (docHeader != null) return Json(new { result = "500", message = "Este documento ya existe." });

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

                return Json(new { result = "200", message = _documentHeader });
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

                using (var db = new RepositorioDocRCEntities())
                {
                    _documentContent = db.DocumentContents.FirstOrDefault(d => d.DocumentHeaderId == documentContent.DocumentHeaderId);

                    if (_documentContent != null)
                    {
                        _documentContent.Body = documentContent.Body;
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
    }
}