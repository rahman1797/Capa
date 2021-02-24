using CAPA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using ClosedXML.Excel;

namespace CAPA.Controllers
{
    public class ToDoController : Controller
    {
        private readonly CapaDbContext context;
        public ToDoController(CapaDbContext context)
        {
            this.context = context;
        }
       
        [HttpPost]
        public IActionResult GetToDos()
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
                /*var ToDoData = (from row in context.Capas select row);*/
                var ToDoData = (from tempCapa in context.capa
                                                 select new
                                                 {
                                                     id = tempCapa.id,
                                                     capa_no = tempCapa.capa_no.ToString(),
                                                     initiation_date = tempCapa.initiation_date.ToString(),
                                                     source = tempCapa.source,
                                                     problem = tempCapa.problem,
                                                     initiator = (from Emp in context.employee where tempCapa.initiator == Emp.id select Emp.display_name).First(), //ini return display_name yang sesuai id di tabel employees
                                                     id_admin = (from Adm in (
                                                                 (from tempAdmin in context.admin
                                                                  select new
                                                                  {
                                                                      id = tempAdmin.id,
                                                                      id_employee = (from Emp in context.employee where tempAdmin.id_employee == Emp.id select Emp.display_name).First(),
                                                                  })
                                                                 )
                                                                 where tempCapa.id_admin == Adm.id
                                                                 select Adm.id_employee).First(),
                                                     id_category = (from Kat in context.category where tempCapa.id_category == Kat.id select Kat.category_name).First(),
                                                     is_proper = tempCapa.is_proper,
                                                     is_active = tempCapa.is_active.ToString(),
                                                     status = tempCapa.status,
                                                     flag = tempCapa.flag,
                                                 });


                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    ToDoData = ToDoData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    ToDoData = ToDoData.Where(m => m.capa_no.ToString().Contains(searchValue)
                                                || m.source.Contains(searchValue)
                                                || m.problem.Contains(searchValue)
                                                || m.initiation_date.ToString().Contains(searchValue)
                                                || m.initiator.ToString().Contains(searchValue)
                                                || m.flag.ToString().Contains(searchValue));
                }
                recordsTotal = ToDoData.Count();
                var data = ToDoData.Skip(skip).Take(pageSize).ToList();
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

       
    }
}
