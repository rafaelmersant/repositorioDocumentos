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

        //[HttpPost]
        //public JsonResult AddDocumentHeader(DocumentHeader documentHeader)
        //{
        //    try
        //    {
        //        if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

        //        using (var db = new RepositorioDocRCEntities())
        //        {
        //            var department = db.Departments.FirstOrDefault(o => o.DeptoName.ToLower() == description.ToLower() && o.DeptoCode == code);
        //            if (department != null) return Json(new { result = "500", message = "Este departamento ya existe." });

        //            db.Departments.Add(new Department
        //            {
        //                DeptoCode = code,
        //                DeptoName = description,
        //                DeptoOwner = owner,
        //                AreaId = areaId,
        //                CreatedDate = DateTime.Now,
        //                CreatedBy = int.Parse(Session["userID"].ToString())
        //            });
        //            db.SaveChanges();
        //        }

        //        return Json(new { result = "200", message = "success" });
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.SendException(ex, $"description: {description}");

        //        return Json(new { result = "500", message = ex.Message });
        //    }
        //}
    }
}