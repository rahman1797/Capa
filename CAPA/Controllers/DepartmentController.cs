using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CAPA.Models;
using System.Linq.Dynamic.Core;

namespace CAPA.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly CapaDbContext _context;

        public DepartmentController(CapaDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            ViewData["pakai_datatables"] = "ya";
            if(TempData["swal"] != null)
            {
                ViewBag.swal = TempData["swal"];
            }
            return View();
        }

        // GET: Departments
        [HttpPost]
        public IActionResult GetDepartments()
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
                var departmentData = (from row in _context.department select row);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    departmentData = departmentData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    departmentData = departmentData.Where(m => m.department_name.Contains(searchValue));
                }
                recordsTotal = departmentData.Count();
                var data = departmentData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("id,department_name,is_active,created_at,updated_at,updated_by")] Department department)
        {
            department.created_at = DateTime.Now;
            department.updated_at = DateTime.Now;
            department.updated_by = "amir";
            if (_context.department.Any(e => e.department_name == department.department_name))
            {
                ModelState.AddModelError("department_name", "Department name already exist");
                return View(department);
            }
            ModelState.Remove("updated_by");
            if (ModelState.IsValid)
            {
                _context.Add(department);
                _context.SaveChanges();
                TempData["swal"] = "Created";
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var department = _context.department.Find(id);
            if (department == null)
            {
                return NotFound();
            }
            ViewBag.is_checked = department.is_active == 1 ? "checked" : "";
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?Linkid=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, [Bind("id,department_name,is_active,created_at,updated_at,updated_by")] Department department)
        {
            if (id != department.id)
            {
                return NotFound();
            }
            Department departmentExist = _context.department.AsNoTracking().FirstOrDefault(x => x.id == department.id);
            if (_context.department.Any(e => e.department_name == department.department_name) && departmentExist.department_name != department.department_name)
            {
                ModelState.AddModelError("department_name", "Department name already exist");
                return View(department);
            }
            ModelState.Remove("updated_by");
            if (ModelState.IsValid)
            {
                try
                {
                    department.updated_at = DateTime.Now;
                    department.updated_by = "amir-update";
                    _context.Update(department);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.id))
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
            return View(department);
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            try
            {
                var department = _context.department.AsNoTracking().FirstOrDefault(x => x.id == ID);

                if (department != null)
                {
                    _context.Remove(department);
                    _context.SaveChanges();
                    return Json(true);
                }
            }
            catch (Exception ex)
            {
            }
            return Json(false);
        }

        private bool DepartmentExists(int? id)
        {
            return _context.department.Any(e => e.id == id);
        }
    }
}
