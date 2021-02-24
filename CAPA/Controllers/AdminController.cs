using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks; 
using CAPA.Models; 
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CAPA.Controllers
{
    public class AdminController : Controller
    {
        private readonly CapaDbContext context;
        public AdminController(CapaDbContext context)
        {
            this.context = context;
        }
        [HttpPost]
        public IActionResult GetAdmins()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var adminData = (from tempAdmin in context.admin
                                  select new
                                  {
                                      id = tempAdmin.id,
                                      id_employee = (from Emp in context.employee where tempAdmin.id_employee == Emp.id select Emp.display_name).First(),
                                      id_employee_spv = (from Emp in context.employee where tempAdmin.id_employee_spv == Emp.id select Emp.display_name).First(),
                                      id_rule = (from Rl in context.rule where tempAdmin.id_rule == Rl.id select Rl.rule_name).First(),
                                      description = tempAdmin.description,
                                      is_active = tempAdmin.is_active.ToString(), //"dd-MMMM-yyyy H:m"
                                      created_at = tempAdmin.created_at,//.ToString(),
                                      updated_at = tempAdmin.updated_at,//.ToString(),
                                      updated_by = tempAdmin.updated_by,
                                  });
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    adminData = adminData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    adminData = adminData.Where(m => 
                                                   m.id_rule.Contains(searchValue)
                                                || m.is_active.Contains(searchValue)
                                                || m.id_employee.Contains(searchValue)
                                                || m.id_employee_spv.Contains(searchValue)
                                                || m.description.Contains(searchValue)
                                                //|| m.created_at.Contains(searchValue)
                                                //|| m.updated_at.Contains(searchValue)
                                                || m.updated_by.Contains(searchValue));
                }
                recordsTotal = adminData.Count();
                var data = adminData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult Index()
        {
            ViewData["pakai_datatables"] = "ya";
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = context.admin.Find(id);
            //var obj = context.Admins.AsNoTracking().FirstOrDefault(x => x.id == id);
            if (obj == null)
            {
                return NotFound();
            }
            context.admin.Remove(obj);
            context.SaveChanges();
            return Json(new
            {
                code = 200,
                message = "success"
            });
            //return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = context.admin.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            ViewBag.IDEmployee = new SelectList(context.employee, nameof(Employee.id), nameof(Employee.display_name));
            ViewBag.IDRule = new SelectList(context.rule, nameof(Rule.id), nameof(Rule.rule_name));
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id,Admin obj)
        {
            if (id != obj.id)
            {
                return NotFound();
            }

            Admin adminExists = context.admin.AsNoTracking().FirstOrDefault(x => x.id == obj.id);
            if (context.admin.Any(e => e.id_employee == obj.id_employee) && adminExists.id_employee != obj.id_employee)
            {
                 
                ModelState.AddModelError("id_employee", "*Admin name already exist");
                ViewBag.IDEmployee = new SelectList(context.employee, nameof(Employee.id), nameof(Employee.display_name));
                ViewBag.IDRule = new SelectList(context.rule, nameof(Rule.id), nameof(Rule.rule_name));
                return View(obj);
            }

            if (obj.id_rule == 1 && obj.id_employee_spv == 0)
            {
                ModelState.AddModelError("id_employee_spv", "*Required Super Admin");
                ViewBag.IDEmployee = new SelectList(context.employee, nameof(Employee.id), nameof(Employee.display_name));
                ViewBag.IDRule = new SelectList(context.rule, nameof(Rule.id), nameof(Rule.rule_name));
                return View(obj);
            } 

            ModelState.Remove("updated_by");
            if (ModelState.IsValid)
            {
                try
                {
                    obj.updated_at = DateTime.Now;
                    obj.updated_by = "amir-Update";
                    context.Update(obj);
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(obj.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["swal"] = "Edited";
                return RedirectToAction(nameof(Index)); 
            }
            return View(obj);
        }

        [HttpGet]
        public IActionResult Create()
        { 
            ViewBag.IDEmployee = new SelectList(context.employee, nameof(Employee.id), nameof(Employee.display_name)); 
            ViewBag.IDRule = new SelectList(context.rule, nameof(Rule.id), nameof(Rule.rule_name)); 
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Admin obj)
        {

            obj.is_active = 1;
            obj.created_at = DateTime.Now;
            obj.updated_at = DateTime.Now;
            obj.updated_by = "amir-Insert";

            var checkAdmin = context.admin.Where(x => x.id_employee == obj.id_employee).FirstOrDefault();
            if (checkAdmin != null)
            {
                ModelState.AddModelError("id_employee", "*Admin name already exist");
                ViewBag.IDEmployee = new SelectList(context.employee, nameof(Employee.id), nameof(Employee.display_name));
                ViewBag.IDRule = new SelectList(context.rule, nameof(Rule.id), nameof(Rule.rule_name));
                return View(obj);
            }

            if (obj.id_rule == 1 && obj.id_employee_spv == 0)
            { 
                ModelState.AddModelError("id_employee_spv", "*Required Super Admin"); 
                ViewBag.IDEmployee = new SelectList(context.employee, nameof(Employee.id), nameof(Employee.display_name));
                ViewBag.IDRule = new SelectList(context.rule, nameof(Rule.id), nameof(Rule.rule_name));
                return View(obj);
            }
            ModelState.Remove("updated_by");
            if (ModelState.IsValid)
            {  
                context.Add(obj);
                context.SaveChanges();
                TempData["swal"] = "Created";
                return RedirectToAction(nameof(Index)); 
            }
            return View(obj);
        }

        private bool AdminExists(int? id)
        {
            return context.admin.Any(e => e.id == id);
        }

    }
}
