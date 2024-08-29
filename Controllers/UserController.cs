﻿using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            try
            {
                Session["employeeID"] = null;
                Session["role"] = null;
                Session["email"] = null;
                Session["hasEmail"] = null;
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    string _pass = Helper.SHA256(password);
                    var _user = db.Users.FirstOrDefault(u => u.EmployeeID == username && u.PasswordHash == _pass);

                    if (_user != null)
                    {
                        Session["employeeID"] = username;
                        Session["role"] = _user.Role;
                        Session["userID"] = _user.Id;

                        Session["hasEmail"] = !string.IsNullOrEmpty(_user.Email) && _user.Email.Contains("@") ? "Yes" : "No";

                        if (!string.IsNullOrEmpty(_user.Email))
                        {
                            Session["email"] = _user.Email;
                        }

                        db.LoginHistories.Add(new LoginHistory
                        {
                            LastLogin = DateTime.Now,
                            UserID = _user.Id
                        });
                        db.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.EmployeeId = username;
                        ViewBag.Message = "Usuario/Contraseña incorrecto.";
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                ViewBag.Message = ex.Message;
            }

            return View();
        }

        public ActionResult UsersList()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");
            if (Session["role"].ToString() != "Admin") return RedirectToAction("Index", "Home");

            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    var users = db.Users.OrderByDescending(o => o.CreatedDate).ToList();
                    return View(users);
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return null;
            }
        }

        public ActionResult RegisterUser()
        {
            if (Session["role"] != null && Session["role"].ToString() != "Admin") return RedirectToAction("Index", "Home");

            ViewBag.Roles = GetRoles();

            return View();
        }

        public void addUser(string email, string employeeID, string pass)
        {
            try
            {
                User user = new User
                {
                    Email = email,
                    EmployeeID = employeeID,
                    PasswordHash = pass,
                    Role = "Registro"
                };

                RegisterUser(user);
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(User user)
        {
            ViewBag.Result = "info";

            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    if (ModelState.IsValid)
                    {
                        var users = db.Users.ToList();

                        //This EmployeeId exists ?
                        var _userEmployeeId = users.FirstOrDefault(u => u.EmployeeID == user.EmployeeID);
                        if (_userEmployeeId != null) throw new Exception("Este código de usuario ya existe en el sistema.");

                        var newUser = new User
                        {
                            CreatedDate = DateTime.Now,
                            IdHash = Guid.NewGuid(),
                            Email = user.Email,
                            EmployeeID = user.EmployeeID,
                            PasswordHash = Helper.SHA256(user.PasswordHash),
                            Role = user.Role
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges();

                        return RedirectToAction("UsersList");
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                ViewBag.Result = "danger";
                ViewBag.Message = ex.Message;
            }

            ViewBag.Roles = GetRoles();

            return View();
        }

        public ActionResult Edit(Guid? IdHash)
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            try
            {
                ViewBag.Roles = GetRoles();

                if (IdHash == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                using (var db = new RepositorioDocRCEntities())
                {
                    User _user = db.Users.FirstOrDefault(u => u.IdHash == IdHash);
                    if (_user == null)
                    {
                        return HttpNotFound();
                    }

                    return View(_user);
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User _user)
        {
            ViewBag.Roles = GetRoles();

            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new RepositorioDocRCEntities())
                    {
                        User user_edit = db.Users.FirstOrDefault(u => u.IdHash == _user.IdHash);

                        if (user_edit != null)
                        {
                            var users = db.Users.ToList();

                            //This EmployeeId exists ?
                            var _userEmployeeId = users.FirstOrDefault(u => u.EmployeeID == _user.EmployeeID);
                            if (_userEmployeeId != null && user_edit.EmployeeID != _user.EmployeeID) throw new Exception("Este código de usuario ya existe en el sistema.");

                            user_edit.Role = _user.Role;
                            user_edit.Email = _user.Email;
                            user_edit.EmployeeID = _user.EmployeeID;
                            //user_edit.PasswordHash = _pass;

                            db.SaveChanges();
                            return RedirectToAction("UsersList", "User");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helper.SendException(ex);

                    ViewBag.Result = "danger";
                    ViewBag.Message = ex.Message;
                }

                return View(_user);
            }

            return View(_user);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    User _user = db.Users.FirstOrDefault(u => u.Id == id);
                    if (_user != null)
                    {
                        db.Users.Remove(_user);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return Json(new { result = "500", message = ex.Message });
            }

            return Json(new { result = "200", message = "Success" });
        }

        private IList<SelectListItem> GetRoles()
        {
            IList<SelectListItem> roles = new List<SelectListItem>
             {
                 new SelectListItem() {Text="Consulta", Value="Consulta"},
                 new SelectListItem() { Text="Admin", Value="Admin"}
             };
            return roles;
        }

        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RecoverPassword(string employeeId, string email)
        {
            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    //This EmployeeId exists ?
                    var _userEmployeeId = db.Users.FirstOrDefault(u => u.EmployeeID == employeeId && u.Email == email);
                    if (_userEmployeeId == null) throw new Exception("Este código/email de usuario no fue encontrado.");

                    var user_edit = db.Users.FirstOrDefault(u => u.EmployeeID == employeeId);
                    string newPassword = Environment.TickCount.ToString().Substring(0, 4);
                    string newPasswordHash = Helper.SHA256(newPassword);

                    user_edit.PasswordHash = newPasswordHash;
                    db.SaveChanges();

                    Helper.SendRecoverPasswordEmail(newPassword, email);

                    return new JsonResult { Data = "200", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("/email"))
                    Helper.SendException(ex);

                return new JsonResult { Data = ex.Message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public ActionResult ChangePassword()
        {
            if (Session["role"] == null) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public JsonResult ChangePassword(string currentPassword, string newPassword)
        {
            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    string employeeId = Session["employeeID"].ToString();
                    string password = Helper.SHA256(currentPassword);

                    var currentUser = db.Users.FirstOrDefault(u => u.EmployeeID == employeeId && u.PasswordHash == password);

                    if (currentUser != null)
                    {
                        currentUser.PasswordHash = Helper.SHA256(newPassword);
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("La contraseña actual es incorrecta, favor verificar.");
                    }

                    return new JsonResult { Data = "200", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return new JsonResult { Data = ex.Message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public string GetEmailByEmployeeId(string employeeId)
        {
            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    var employee = db.Users.FirstOrDefault(e => e.EmployeeID == employeeId);
                    if (employee != null)
                    {
                        return employee.Email;
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Helper.SendRawEmail("rafaelmersant@sagaracorp.com", "Exception in GetEmailByEmployeeId", ex.ToString());
                }
                catch (Exception exq)
                {
                    Console.WriteLine(exq.ToString());
                }

                return ex.Message;
            }

            return string.Empty;
        }

        public List<SelectListItem> GetUsers()
        {
            List<SelectListItem> users = new List<SelectListItem>();

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RepoDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            users.Add(new SelectListItem { Text = "Seleccionar...", Value = "" });

                            while (reader.Read())
                            {
                                users.Add(new SelectListItem
                                {
                                    Text = reader.GetString(reader.GetOrdinal("EmployeeName")).Trim(),
                                    Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
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

            return users;
        }
    }
}