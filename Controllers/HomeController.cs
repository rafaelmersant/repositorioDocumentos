using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RepositorioDocumentos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["role"] == null) return RedirectToAction("Login", "User");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult IsSessionExpired()
        {
            bool isExpired = Session["role"] == null;
            return Json(new { expired = isExpired }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestEmailAdmin()
        {
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            try
            {
                string content = "Su nueva contraseña es: <b>" + "testingpassword" + "</b>";

                SmtpClient smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["smtpClient"],
                    Port = int.Parse(ConfigurationManager.AppSettings["PortMail"]),
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["usrEmail"], ConfigurationManager.AppSettings["pwdEmail"]),
                    EnableSsl = true,
                };

                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.Body = content;
                message.Subject = "PRUEBA DE SISTEMA DE REPOSITORIO DE DOCUMENTOS";
                message.To.Add(new MailAddress("rafaelmersant@sagaracorp.com"));

                string address = ConfigurationManager.AppSettings["EMail"];
                string displayName = ConfigurationManager.AppSettings["EMailName"];
                message.From = new MailAddress(address, displayName);

                smtp.Send(message);

                return Content("Email Enviado!");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}