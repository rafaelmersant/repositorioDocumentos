using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using RepositorioDocumentos.Models;

namespace RepositorioDocumentos.App_Start
{
    public class Helper
    {
        public static bool SendRawEmail(string emailto, string subject, string body)
        {
            try
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["smtpClient"],
                    Port = int.Parse(ConfigurationManager.AppSettings["PortMail"]),
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["usrEmail"], ConfigurationManager.AppSettings["pwdEmail"]),
                    EnableSsl = false,
                };

                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.Body = body;
                message.Subject = subject;
                message.To.Add(new MailAddress(emailto));

                string address = ConfigurationManager.AppSettings["EMail"];
                string displayName = ConfigurationManager.AppSettings["EMailName"];
                message.From = new MailAddress(address, displayName);


                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return false;
            }
        }

        public static void SendException(Exception ex, string extraInfo = "")
        {
            try
            {
                string _sentry = ConfigurationManager.AppSettings["sentry_dsn"];
                string _environment = ConfigurationManager.AppSettings["sentry_environment"];

                var ravenClient = new SharpRaven.RavenClient(_sentry);
                ravenClient.Environment = _environment;

                var exception = new SharpRaven.Data.SentryEvent(ex);

                if (!string.IsNullOrEmpty(extraInfo))
                    exception.Extra = extraInfo;

                ravenClient.Capture(exception);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.ToString());
            }
        }

        public static void SendException(string message)
        {
            try
            {
                string _sentry = ConfigurationManager.AppSettings["sentry"];
                string _environment = ConfigurationManager.AppSettings["sentry_environment"];

                var ravenClient = new SharpRaven.RavenClient(_sentry);
                ravenClient.Environment = _environment;
                ravenClient.Capture(new SharpRaven.Data.SentryEvent(message));

            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.ToString());
            }
        }

        public static string SHA256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));

            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("X2"));
            }
            return hash.ToString();
        }
             
        public static List<SelectListItem> GetDepartments()
        {
            List<SelectListItem> departments = new List<SelectListItem>();
            departments.Add(new SelectListItem { Text = "Todos", Value = "0" });

            try
            {
                using (var db = new RepositorioDocRCEntities())
                {
                    var _departments = db.Departments
                                         .Select(s => new { s.DeptoCode, s.DeptoName })
                                         .Distinct()
                                         .OrderBy(o => o.DeptoName)
                                         .ToList();

                    foreach (var depto in _departments)
                        departments.Add(new SelectListItem { Text = depto.DeptoName, Value = depto.DeptoCode.ToString() });
                }
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);
            }

            return departments;
        }

        public static bool SendRecoverPasswordEmail(string newPassword, string email)
        {
            try
            {
                string content = "Su nueva contraseña es: <b>" + newPassword + "</b>";

                SmtpClient smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["smtpClient"],
                    Port = int.Parse(ConfigurationManager.AppSettings["PortMail"]),
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["usrEmail"], ConfigurationManager.AppSettings["pwdEmail"]),
                    EnableSsl = false,
                };

                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.Body = content;
                message.Subject = "NUEVA CONTRASEÑA SISTEMA DE GESTION Y ADMINISTRACION DE VACACIONES";
                message.To.Add(new MailAddress(email));

                string address = ConfigurationManager.AppSettings["EMail"];
                string displayName = ConfigurationManager.AppSettings["EMailName"];
                message.From = new MailAddress(address, displayName);

                smtp.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                Helper.SendException(ex);

                return false;
            }
        }

        public static void GetElapsedTime(DateTime from_date, DateTime to_date,
        out int years, out int months, out int days, out int hours,
        out int minutes, out int seconds, out int milliseconds)
        {
            // If from_date > to_date, switch them around.
            if (from_date > to_date)
            {
                GetElapsedTime(to_date, from_date,
                    out years, out months, out days, out hours,
                    out minutes, out seconds, out milliseconds);
                years = -years;
                months = -months;
                days = -days;
                hours = -hours;
                minutes = -minutes;
                seconds = -seconds;
                milliseconds = -milliseconds;
            }
            else
            {
                // Handle the years.
                years = to_date.Year - from_date.Year;

                // See if we went too far.
                DateTime test_date = from_date.AddMonths(12 * years);
                if (test_date > to_date)
                {
                    years--;
                    test_date = from_date.AddMonths(12 * years);
                }
                // Add months until we go too far.
                months = 0;
                while (test_date <= to_date)
                {
                    months++;
                    test_date = from_date.AddMonths(12 * years + months);
                }
                months--;

                // Subtract to see how many more days,
                // hours, minutes, etc. we need.
                from_date = from_date.AddMonths(12 * years + months);
                TimeSpan remainder = to_date - from_date;
                days = remainder.Days;
                hours = remainder.Hours;
                minutes = remainder.Minutes;
                seconds = remainder.Seconds;
                milliseconds = remainder.Milliseconds;
            }
        }
    }
}