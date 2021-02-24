using CAPA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace CAPA.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CapaDbContext context;

        public CategoryController(CapaDbContext context)
        {
            this.context = context;
        }

        // GET: Category
        public IActionResult Index()
        {
            ViewData["pakai_datatables"] = "ya";
            return View();
        }

        [HttpPost]
        public IActionResult GetDatatableCategory()
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
                var categoryData = (from category in context.category
                                    select new
                                    {
                                        id = category.id,
                                        category_name = category.category_name,
                                        description = category.description,
                                        is_active = category.is_active,
                                        created_at = DateTime.Parse(category.created_at.ToString()).ToString("yyyy-MM-dd HH:mm"),
                                    });
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    if(sortColumn == "created_at")
                    {
                        //categoryData = categoryData.OrderByDescending(e => e.created_at);
                    } else
                    {
                        categoryData = categoryData.OrderBy(sortColumn + " " + sortColumnDirection);
                    }
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    categoryData = categoryData.Where(m => m.category_name.Contains(searchValue)
                                                || m.description.Contains(searchValue));
                }
                recordsTotal = categoryData.Count();
                var data = categoryData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult Create([Bind("category_name,description")] Category category)
        {
            var checkCategoryName = context.category.Where(x => x.category_name == category.category_name).FirstOrDefault();
            if(checkCategoryName != null)
            {
                return Json(new
                {
                    metaData = new { code = 402, message = "Kategori dengan nama "+ category.category_name + " sudah ada."},
                    response = ""
                });
            }

            Category addCategory = new Category { category_name = category.category_name, description = category.description, is_active = 1, created_at = DateTime.Now, updated_by = "admin" };
            context.category.Add(addCategory);
            context.SaveChanges();
            return Json(new
            {
                metaData = new { code = 200, message = "OK" },
                response = addCategory
            });
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await context.category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,Category category)
        {
            var data = context.category.Where(x => x.id == id).FirstOrDefault();

            if (data != null)
            {
                var checkCategoryName = context.category.Where(x => x.category_name == category.category_name).FirstOrDefault();
                if (checkCategoryName != null)
                {
                    if(category.category_name != data.category_name) { 
                        TempData["Failed"] = "Kategori dengan nama " + category.category_name + " sudah ada.";
                        return View(category);
                    }
                }
                    

                data.category_name = category.category_name;
                data.description = category.description;
                data.is_active = category.is_active;
                data.updated_at = DateTime.Now;
                data.updated_by = "admin";

                context.SaveChanges();
            }

            TempData["Message"] = "Data berhasil di update.";

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            var deleteCategory = context.category.ToList().Find(i => i.id == id);
            if (deleteCategory != null)
            {
                context.category.Remove(deleteCategory);
                context.SaveChanges();

            }

            return Json(new
            {
                metaData = new { code = 200, message = "OK"  },
                response = ""
            });
        }

        private bool CategoryExists(int id)
        {
            return context.category.Any(e => e.id == id);
        }
    }
}
