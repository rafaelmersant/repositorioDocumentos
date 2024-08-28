using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System.Data.Entity.Infrastructure;

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
                ViewBag.Employees = GetEmployees();

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
        public JsonResult AddDepartment(int code, short areaId, string description, string reference, string owner)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    string _reference = string.IsNullOrEmpty(reference) ? "" : reference.ToUpper();

                    var department = db.Departments.FirstOrDefault(o => o.DeptoName.ToLower() == description.ToLower() && o.DeptoCode == code);
                    if (department != null) return Json(new { result = "500", message = "Este departamento ya existe." });

                    var _reference_ = db.Departments.FirstOrDefault(o => o.Reference.ToLower() == _reference.ToLower());
                    if (_reference_ != null && !string.IsNullOrEmpty(_reference)) return Json(new { result = "500", message = "Esta referencia ya existe." });

                    db.Departments.Add(new Department
                    {
                        DeptoCode = code,
                        DeptoName = description,
                        DeptoOwner = owner,
                        Reference = _reference,
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
        public JsonResult UpdateDepartment(int code, short areaId, string description, string reference, string owner)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    string _reference = string.IsNullOrEmpty(reference) ? "" : reference.ToUpper();

                    var department = db.Departments.FirstOrDefault(o => o.DeptoCode == code);
                    if (department == null) return Json(new { result = "500", message = "Departamento no encontrado." });
                    if (department.DeptoName.ToLower() == description.ToLower() 
                        && department.AreaId == areaId
                        && department.DeptoOwner == owner
                        && department.DeptoCode != code) return Json(new { result = "500", message = "Este departamento ya existe." });

                    var _reference_ = db.Departments.FirstOrDefault(o => o.Reference.ToLower() == _reference.ToLower() && o.DeptoCode != code);
                    if (_reference_ != null && !string.IsNullOrEmpty(_reference)) return Json(new { result = "500", message = "Esta referencia ya existe." });

                    department.DeptoName = description;
                    department.Reference = _reference;
                    department.AreaId = areaId;
                    department.DeptoOwner = owner;
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

        [HttpPost]
        public JsonResult getDepartmentsByDirectorateId(int directorateId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                var departments = GetDepartamentosByDirectorate(directorateId);

                return Json(new { result = "200", message = departments });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"directorateId: {directorateId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult getDepartmentsByDirectorateIdReference(string directorateReference)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");
                using (var db = new RepositorioDocRCEntities())
                {
                    var directorate = db.Directorates.FirstOrDefault(d => d.Reference == directorateReference);
                    var departments = GetDepartamentosByDirectorateReference(directorate.Id);
                    return Json(new { result = "200", message = departments });
                }

            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"directorateReference: {directorateReference}");

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

        public List<SelectListItem> GetDepartamentosByDirectorate(int directorateId = 0)
        {
            List<SelectListItem> departamentos = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                departamentos.Add(new SelectListItem { Text = "Seleccionar Departamento", Value = "" });
                var _departamentos = db.Departments.ToArray();
                if (directorateId > 0) _departamentos = _departamentos.Where(d => d.Area.DirectorateId == directorateId).ToArray();

                foreach (var item in _departamentos)
                    departamentos.Add(new SelectListItem { Text = item.DeptoName, Value = item.DeptoCode.ToString() });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return departamentos;
        }

        public List<SelectListItem> GetDepartamentosByDirectorateReference(int directorateId = 0)
        {
            List<SelectListItem> departamentos = new List<SelectListItem>();

            try
            {
                var db = new RepositorioDocRCEntities();
                departamentos.Add(new SelectListItem { Text = "Seleccionar Departamento", Value = "" });
                var _departamentos = db.Departments.ToArray();
                if (directorateId > 0) _departamentos = _departamentos.Where(d => d.Area.DirectorateId == directorateId).ToArray();

                foreach (var item in _departamentos)
                    departamentos.Add(new SelectListItem { Text = item.DeptoName, Value = item.Reference });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return departamentos;
        }

        public List<SelectListItem> GetEmployees()
        {
            List<SelectListItem> employees = new List<SelectListItem>();

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RepoDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetEmployees", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            employees.Add(new SelectListItem { Text = "Seleccionar...", Value = "" });

                            while (reader.Read())
                            {
                                employees.Add(new SelectListItem
                                {
                                    Text = reader.GetString(reader.GetOrdinal("EmployeeName")).Trim(),
                                    Value = reader.GetString(reader.GetOrdinal("EmployeeName")).Trim()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return employees;
        }

        public List<SelectListItem> GetEmployeesWithID()
        {
            List<SelectListItem> employees = new List<SelectListItem>();

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RepoDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetEmployees", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            employees.Add(new SelectListItem { Text = "Seleccionar...", Value = "" });

                            while (reader.Read())
                            {
                                employees.Add(new SelectListItem
                                {
                                    Text = reader.GetString(reader.GetOrdinal("EmployeeName")).Trim(),
                                    Value = reader.GetInt32(reader.GetOrdinal("EmployeeId")).ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return employees;
        }
    }
}