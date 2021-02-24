using CAPA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace CAPA.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly CapaDbContext context;
        public EmployeeController(CapaDbContext context)
        {
            this.context = context;
        }
       
        [HttpPost]
        public IActionResult GetEmployees()
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
                var employeeData = (from tempcustomer in context.employee select tempcustomer);
              
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    employeeData = employeeData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    employeeData = employeeData.Where(m => m.display_name.Contains(searchValue)
                                                || m.email.Contains(searchValue)
                                                || m.is_active.ToString().Contains(searchValue)
                                                /*|| m.created_at.ToString().Contains(searchValue)
                                                || m.updated_at.ToString().Contains(searchValue)*/
                                                || m.updated_by.Contains(searchValue));
                }
                recordsTotal = employeeData.Count();
                var data = employeeData.Skip(skip).Take(pageSize).ToList();
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

        // GET-Create
        public IActionResult Create()
        {
            ViewData["pakai_datatables"] = "ya";
            return View();
        }
        // POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (context.employee.Any(e => e.email == obj.email))
                    {
                        ModelState.AddModelError("email", "Email sudah terdaftar");
                        return View(obj);
                    }
                    if (obj.id == null || obj.id == 0)
                    {
                        obj.is_active = 1;
                        obj.created_at = DateTime.Now;
                        obj.updated_at = DateTime.Now;
                        obj.updated_by = "Inggrid";
                        context.employee.Add(obj);
                        if (obj.display_name != null && obj.email != null)
                        {
                            context.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
                /*ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");*/
            }
            return RedirectToAction("Index");
        }

        // POST-Delete
        [HttpPost]
        public IActionResult Delete(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = context.employee.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            context.employee.Remove(obj);
            context.SaveChanges();

            return Json(new
            {
                code = 200,
                message = "success"
            });
        }
        

        //GET - Update
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var employee = context.employee.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewBag.is_checked = employee.is_active == 1 ? "checked" : ""; //untuk is_active
            return View("Update", employee);
        }
        // POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Employee obj)
        {
            if (obj != null)
            {
                obj.updated_at = DateTime.Now;
                obj.updated_by = "Inggrid";
            }

            context.Update(obj);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
