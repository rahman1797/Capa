using CAPA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace CAPA.Controllers
{
    public class LogController : Controller
    {
        private readonly CapaDbContext context;
        public LogController(CapaDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        public IActionResult SaveLog(LogLogin logs)
        {
            var emailLogin = User.Identity.Name;

            //Get Id employee & Rule
            var getStuff = (from db_employee in context.employee
                            join db_admin in context.admin
                            on db_employee.id equals db_admin.id_employee
                            select new
                            {
                                id_employee = db_employee.id,
                                rule = db_admin.id_rule,
                                email = db_employee.email
                            }).Where(db_log => db_log.email == emailLogin).ToList();

            var id_employee = getStuff[0].id_employee;
            var rule = getStuff[0].rule;

            //Get IP Address
            var IpAddress = Request.HttpContext.Connection.RemoteIpAddress;

            //Get Browser
            var browser = Request.Headers["User-Agent"].ToString();

            //Save Log
            var logData = (from db_log in context.log_login
                           select new
                           {
                               id = db_log.id,
                               id_employee = db_log.id_employee,
                               deleted_at = db_log.deleted_at
                           }).Where(db_log => db_log.id_employee == id_employee).ToList();
            int count_deletedAt = logData.Count();
            int session_id = 0;
            if (count_deletedAt > 0)
            {
                var deleted_at = logData[count_deletedAt - 1].deleted_at;
                var id = logData[count_deletedAt - 1].id;

                if (deleted_at != null)
                {
                    logs.id_employee = Convert.ToInt32(id_employee);
                    logs.rule = (from RuleName in context.rule where RuleName.id == rule select RuleName.rule_name).First();
                    logs.last_login = DateTime.Now;
                    logs.alamat_ip = IpAddress.ToString();
                    logs.browser = browser;
                    logs.created_at = DateTime.Now;
                    context.log_login.Add(logs);
                    context.SaveChanges();

                    session_id = logs.id;
                    HttpContext.Session.SetInt32("id", Convert.ToInt32(session_id));
                    HttpContext.Session.SetInt32("id_employee", Convert.ToInt32(id_employee));
                }
                else
                {
                    session_id = id;
                    HttpContext.Session.SetInt32("id", Convert.ToInt32(session_id));
                    HttpContext.Session.SetInt32("id_employee", Convert.ToInt32(id_employee));
                }
            }
            else
            {
                logs.id_employee = Convert.ToInt32(id_employee);
                logs.rule = (from RuleName in context.rule where RuleName.id == rule select RuleName.rule_name).First();
                logs.last_login = DateTime.Now;
                logs.alamat_ip = IpAddress.ToString();
                logs.browser = browser;
                logs.created_at = DateTime.Now;
                context.log_login.Add(logs);
                context.SaveChanges();

                session_id = logs.id;
                HttpContext.Session.SetInt32("id", Convert.ToInt32(session_id));
                HttpContext.Session.SetInt32("id_employee", Convert.ToInt32(id_employee));
            }
                                                
            return Redirect("~/Home/Index");
        }

        public IActionResult UpdateLog(LogLogin logs)
        {
            var session_id = HttpContext.Session.GetInt32("id");

            var logData = (from db_log in context.log_login
                           select new
                           {
                               id = db_log.id,
                               id_employee = db_log.id_employee,
                               rule = db_log.rule,
                               last_login = db_log.last_login,
                               alamat_ip = db_log.alamat_ip,
                               browser = db_log.browser,
                               created_at = db_log.created_at,
                               deleted_at = db_log.deleted_at
                           }).Where(db_log => db_log.id == session_id).ToList();
            int count_deletedAt = logData.Count();
            if (count_deletedAt > 0)
            {
                var id = logData[count_deletedAt - 1].id;
                var id_employee = logData[count_deletedAt - 1].id_employee;
                var rule = logData[count_deletedAt - 1].rule;
                var last_login = logData[count_deletedAt - 1].last_login;
                var IpAddress = logData[count_deletedAt - 1].alamat_ip;
                var browser = logData[count_deletedAt - 1].browser;
                var created_At = logData[count_deletedAt - 1].created_at;                
                var deleted_at = logData[count_deletedAt - 1].deleted_at;

                if (deleted_at == null)
                {
                    logs.id = id;
                    logs.id_employee = id_employee;
                    logs.rule = rule;
                    logs.last_login = last_login;
                    logs.alamat_ip = IpAddress;
                    logs.browser = browser;
                    logs.created_at = created_At;
                    logs.deleted_at = DateTime.Now;
                    context.log_login.Update(logs);
                    context.SaveChanges();
                }
            }
            return Redirect("~/");
        }

        public IActionResult CheckSession()
        {
            var session_id = HttpContext.Session.GetInt32("id");
            var logData = (from db_log in context.log_login
                           select new
                           {
                               id = db_log.id
                           }).Where(db_log => db_log.id == session_id).ToList();
            var count_logData = logData.Count();
            if(count_logData == 0)
            {
                var jsonData = new {guest = true};
                return Json(jsonData);
            }
            else
            {
                return View("");
            }            
        }
    }
}
