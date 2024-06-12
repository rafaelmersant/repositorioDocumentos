using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;

namespace RepositorioDocumentos.Controllers
{
    public class DepartamentoController : Controller
    {
        // GET: Departamento
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                ViewBag.Areas = new AreaController().GetAreas();

                var db = new RepositorioDocRCEntities();

                var departments = db.Departments.OrderBy(o => o.DeptoName).ToList();
                return View(departments);

            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        [HttpPost]
        public JsonResult AddDepartment(int code, short areaId, string description, string owner)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var department = db.Departments.FirstOrDefault(o => o.DeptoName.ToLower() == description.ToLower() && o.DeptoCode == code);
                    if (department != null) return Json(new { result = "500", message = "Este departamento ya existe." });

                    db.Departments.Add(new Department
                    {
                        DeptoCode = code,
                        DeptoName = description,
                        DeptoOwner = owner,
                        AreaId = areaId,
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
        public JsonResult UpdateDepartment(int code, short areaId, string description, string owner)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var departament = db.Departments.FirstOrDefault(o => o.DeptoCode == code);
                    if (departament == null) return Json(new { result = "500", message = "Departamento no encontrado." });
                    if (departament.DeptoName.ToLower() == description.ToLower() 
                        && departament.AreaId == areaId
                        && departament.DeptoOwner == owner
                        && departament.DeptoCode != code) return Json(new { result = "500", message = "Este departamento ya existe." });

                    departament.DeptoName = description;
                    departament.AreaId = areaId;
                    departament.DeptoOwner = owner;
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"Code: {code} | description: {description}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteDepartment(int code)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var department = db.Departments.FirstOrDefault(o => o.DeptoCode == code);
                    if (department == null) return Json(new { result = "500", message = "Este departamento no existe." });

                    db.Departments.Remove(department);
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"code: {code}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult getDepartmentsByAreaId(int areaId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                var departments = GetDepartamentos(areaId);

                return Json(new { result = "200", message = departments });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"areaId: {areaId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        public List<SelectListItem> GetDepartamentos(int areaId = 0)
        {
            List<SelectListItem> departamentos = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                departamentos.Add(new SelectListItem { Text = "Seleccionar Departamento", Value = "" });
                var _departamentos = db.Departments.ToArray();
                if (areaId > 0) _departamentos = _departamentos.Where(d => d.AreaId == areaId).ToArray();

                foreach (var item in _departamentos)
                    departamentos.Add(new SelectListItem { Text = item.DeptoName, Value = item.DeptoCode.ToString() });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return departamentos;
        }
    }
}