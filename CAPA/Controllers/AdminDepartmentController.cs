using CAPA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http;

namespace CAPA.Controllers
{

    public class AdminDepartmentController : Controller
    {
        private readonly CapaDbContext context;
        public AdminDepartmentController(CapaDbContext context)
        {
            this.context = context;
        }

        public string EmployeeData(int? ID)
        {
            var query = from data in context.employee
                        where (data.id == ID)
                        select data.display_name;
            return query.First();
        }
        public IQueryable DepartmentData(int? ID)
        {
            var query = from data in context.department
                        where (data.id == ID)
                        select data.department_name;
            return query;
        }

        [HttpPost]
        public IActionResult GetAdminDepartment()
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
                var employeeData = (from db in context.admin_department
                                    select new
                                    {
                                        id = db.id,
                                        id_department = (from Dep in context.department where Dep.id == db.id_department select Dep.department_name).First(),
                                        id_employee = (from Emp in context.employee where Emp.id == db.id_employee select Emp.display_name).First(),
                                        is_active = db.is_active,
                                        create_at = db.created_at,
                                        update_at = db.updated_at,
                                        update_by = db.updated_by
                                    });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    employeeData = employeeData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    employeeData = employeeData.Where(m => m.id_department.Contains(searchValue)
                    || m.id_employee.Contains(searchValue) || m.update_by.Contains(searchValue) );
                }
                /*|| m.update_at.ToString("dd/MMM/yyyy HH:mm").Contains(searchValue)*/
                recordsTotal = employeeData.Count();
                
                var data = employeeData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult Index()
        {
            ViewData["pakai_datatables"] = "ya";

            return View();
        }

        public IActionResult CreatePage()
        {
            List<Employee> ListEmployee = new List<Employee>();
            List<Department> ListDepartment = new List<Department>();

            ListEmployee = (from Emp in context.employee
                            select Emp).ToList();
            ListEmployee.Insert(0, new Employee { id = 0, display_name = "Select Employee" });
            ViewBag.ListOfEmployee = ListEmployee;

            ListDepartment = (from Dep in context.department
                              select Dep).ToList();
            ListDepartment.Insert(0, new Department { id = 0, department_name = "Select Department" });
            ViewBag.ListOfDepartment = ListDepartment;

            return View("CreateAdminDepartment");
        }

        [HttpPost]
        public IActionResult Create(AdminDepartment obj)
        {
            var result = "";
            
            if((obj.id_department != 0) && (obj.id_employee != 0))
            {
                var session_id = HttpContext.Session.GetInt32("id_employee");
                obj.created_at = DateTime.Now;
                obj.updated_at = DateTime.Now;
                obj.updated_by = EmployeeData(session_id);

                context.admin_department.Add(obj);
                context.SaveChanges();
                result = "success";
            }   
            return Json(result);
        }


        public IActionResult EditPage(int ID)
        {
            List<Employee> ListEmployee = new List<Employee>();
            List<Department> ListDepartment = new List<Department>();

            ListEmployee = (from Emp in context.employee
                            select Emp).ToList();
            ListEmployee.Insert(0, new Employee { id = 0, display_name = "Select Employee" });
            ViewBag.ListOfEmployee = ListEmployee;

            ListDepartment = (from Dep in context.department
                              select Dep).ToList();
            ListDepartment.Insert(0, new Department { id = 0, department_name = "Select Department" });
            ViewBag.ListOfDepartment = ListDepartment;

            var query = (from AD in context.admin_department
                         where AD.id == ID
                         select AD).FirstOrDefault();

            return View("EditAdminDepartment", query);
        }

        public IActionResult Edit(AdminDepartment obj)
        {
            var result = "error";
            if (obj.id != 0 )
            {
                var query = (from AD in context.admin_department
                             where AD.id == obj.id
                             select AD).FirstOrDefault();
                if (query != null)
                {
                    var session_id = HttpContext.Session.GetInt32("id_employee");
                    query.id_department = obj.id_department;
                    query.id_employee = obj.id_employee;
                    query.updated_at = DateTime.Now;
                    query.is_active = obj.is_active;
                    query.updated_by = EmployeeData(session_id);
                    context.admin_department.Update(query);
                    context.SaveChanges();
                    result = "success";
                }
            }

            return Json(result);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = "";

            if (id != null)
            {
                context.admin_department.Remove(context.admin_department.Find(id));
                context.SaveChanges();
                result = "Deleted";
            }
            else
            {
                result = "Data not found";
            }
            
            return Json(result);
        }
    }
}
