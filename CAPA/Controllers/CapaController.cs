using CAPA.Models;
using CAPA.ViewModel;
using EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace CAPA.Controllers
{
    public class CapaController : BaseController
    {
        private readonly CapaDbContext context;
        private readonly IEmailSender _emailSender;
        private string email, nameAdm, nameInit;
        public CapaController(CapaDbContext context, IEmailSender emailSender)
        {
            this.context = context;
            this._emailSender = emailSender;
        }
              
        [HttpGet]
        public IActionResult Create()
        {
            var adminData = (from tempAdmin in context.admin
                             select new
                             {
                                 id = tempAdmin.id,
                                 id_employee = (from Emp in context.employee where tempAdmin.id_employee == Emp.id select Emp.display_name).First(), 
                             }); 
            ViewBag.IDAdmin = new SelectList(adminData, "id", "id_employee");
            ViewBag.IDCategory = new SelectList(context.category, nameof(Category.id), nameof(Category.category_name));
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Capa obj)
        {
            int maxNumber = 1;
            var numbers = context.capa.DefaultIfEmpty().Max(p => p == null ? 0 : p.capa_no);
            if (numbers > 0  )
            {
                maxNumber = numbers+1;
            }
            obj.flag = 1;

            var sessionemail_id = User.Identity.Name;
            var getStuff = (from db_employee in context.employee
                            select new
                            {
                                id_employee = db_employee.id,
                                email = db_employee.email
                            }).Where(db_log_login => db_log_login.email.Contains(sessionemail_id)).ToList();
            var id_employee = getStuff[0].id_employee;
            obj.initiator = Convert.ToInt32(id_employee);

            obj.initiation_date = DateTime.Now;
            obj.capa_no = maxNumber;
            var AdminData = (from tempAdmin in context.admin where tempAdmin.id == obj.id_admin
                             select new
                             {
                                 emailAdmin = (from Emp in context.employee where tempAdmin.id_employee == Emp.id select Emp.email).First(),
                                 nameAdmin = (from Emp in context.employee where tempAdmin.id_employee == Emp.id select Emp.display_name).First(),
                             });
            foreach (var a in AdminData)
            {
                email = a.emailAdmin;
                nameAdm = a.nameAdmin;
            }

            var InitiatorData = (from tempEmp in context.employee
                                 where tempEmp.id == obj.initiator
                                 select new
                                 {
                                     display_name = tempEmp.display_name,
                                 }).FirstOrDefault() ; 
            nameInit = InitiatorData.display_name;  

            if (ModelState.IsValid)
            { 
                context.Add(obj);  
                var message = new Message(new string[] {email} ,  "New CAPA Notification" , "Dear "+nameAdm+", there is new CAPA notification from "+ nameInit+" with No. CAPA "+obj.capa_no, null); 
                _emailSender.SendEmail(message);

                context.SaveChanges();
                TempData["swal"] = "Created";
                return RedirectToAction(nameof(Index)); 
            }
            return View(obj);
        }
    }
}
