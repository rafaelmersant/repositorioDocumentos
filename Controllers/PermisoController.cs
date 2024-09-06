﻿using RepositorioDocumentos.App_Start;
using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.ViewModels;

namespace RepositorioDocumentos.Controllers
{
    public class PermisoController : Controller
    {
        [HttpPost]
        public JsonResult AddPermission(int documentHeaderId, int userId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var permission = db.DocumentPermissions.FirstOrDefault(o => o.DocumentHeaderId == documentHeaderId && o.UserId == userId);
                    if (permission != null) return Json(new { result = "500", message = "Este registro ya existe." });

                    db.DocumentPermissions.Add(new DocumentPermission
                    {
                        DocumentHeaderId = documentHeaderId,
                        UserId = userId,
                        CreatedDate = DateTime.Now,
                        CreatedBy = int.Parse(Session["userID"].ToString())
                    });
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"documentHeaderId: {documentHeaderId} | userId: {userId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdatePermission(int id, int documentHeaderId, int userId)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var permission = db.DocumentPermissions.FirstOrDefault(o => o.Id == id);
                    if (permission == null) return Json(new { result = "500", message = "Registro no encontrado." });
                    if (permission.UserId == userId
                        && permission.DocumentHeaderId == documentHeaderId
                        && permission.Id != id) return Json(new { result = "500", message = "Este registro ya existe." });

                    permission.UserId = userId;
                    permission.CreatedDate = DateTime.Now;
                    permission.CreatedBy = int.Parse(Session["userID"].ToString());
                    db.SaveChanges();
                }

                return Json(new { result = "200", message = "success" });
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"Id: {id} | userId: {userId}");

                return Json(new { result = "500", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePermission(int id)
        {
            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                using (var db = new RepositorioDocRCEntities())
                {
                    var permission = db.DocumentPermissions.FirstOrDefault(o => o.Id == id);
                    if (permission == null) return Json(new { result = "500", message = "Este registro no existe." });
                    
                    db.DocumentPermissions.Remove(permission);
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
        public JsonResult GetPermissions(int documentHeaderId)
        {
            List<UserPermission> users = new List<UserPermission>();

            try
            {
                if (Session["userID"] == null) throw new Exception("505: Por favor intente logearse de nuevo en el sistema. (La Sesión expiró)");

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RepoDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetPermissions", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@documentHeaderId", documentHeaderId));

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserPermission
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    UserName = reader.GetString(reader.GetOrdinal("EmployeeName")).Trim(),
                                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")).ToString("dd/MM/yyyy")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex, $"documentHeaderId: {documentHeaderId}");

                return Json(new { result = "500", message = ex.Message });
            }

            return Json(new { result = "200", message = users });
        }
    }
}