using CAPA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;

namespace CAPA.Controllers
{
    public class ReportController : Controller
    {
        private readonly CapaDbContext context;

        public ReportController(CapaDbContext context)
        {
            this.context = context;
        }

        public IActionResult CAPA()
        {
            ViewData["pakai_datatables"] = "ya";
            ViewBag.initiators = context.employee.ToList();
            ViewBag.categories = context.category.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult GetDatatableCapa()
        {
            try
            {
                var filter_initiator = Request.Form["filter_initiator"].FirstOrDefault();
                var filter_initiation_date = Request.Form["filter_initiation_date"].FirstOrDefault();
                var filter_category = Request.Form["filter_category"].FirstOrDefault();
                var filter_source = Request.Form["filter_source"].FirstOrDefault();

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var capaData = context.capa.GroupJoin(
                        context.employee,
                        capa => capa.initiator,
                        employee => employee.id,
                        (capa, employee) => new { capa, employee }
                    )
                    .SelectMany(
                    x => x.employee.DefaultIfEmpty(),
                    (capa, employee) => new
                    {
                        capa_no = capa.capa.capa_no,
                        source = capa.capa.source,
                        problem = capa.capa.problem,
                        initiation_date = capa.capa.initiation_date,
                        initiator = capa.capa.initiator,
                        id_category = capa.capa.id_category,
                        initiator_name = employee.display_name
                    }
                );

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    if (sortColumn == "initiation_date")
                    {
                        //capaData = capaData.OrderByDescending(e => e.created_at);
                    }
                    else
                    {
                        capaData = capaData.OrderBy(sortColumn + " " + sortColumnDirection);
                    }
                }

                if (!string.IsNullOrEmpty(filter_initiator))
                {
                    capaData = capaData.Where(m => m.initiator == int.Parse(filter_initiator));
                }

                if (!string.IsNullOrEmpty(filter_initiation_date))
                {
                    string[] initiation_date = filter_initiation_date.Split(" - ");
                    DateTime initiation_from = DateTime.ParseExact(initiation_date[0], "dd/MM/yyyy", null);
                    DateTime initiation_to = DateTime.ParseExact(initiation_date[1], "dd/MM/yyyy", null);

                    capaData = capaData.Where(m => m.initiation_date >= initiation_from && m.initiation_date <= initiation_to);
                }

                if (!string.IsNullOrEmpty(filter_category))
                {
                    capaData = capaData.Where(m => m.id_category == int.Parse(filter_category));
                }

                if (!string.IsNullOrEmpty(filter_source))
                {
                    capaData = capaData.Where(m => m.source.Contains(filter_source));
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    capaData = capaData.Where(m => m.source.Contains(searchValue)
                                                || m.problem.Contains(searchValue));
                }
                recordsTotal = capaData.Count();
                var data = capaData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult ExportExcelCAPA()
        {
            var filter_initiator = Request.Form["filter_initiator"].FirstOrDefault();
            var filter_initiation_date = Request.Form["filter_initiation_date"].FirstOrDefault();
            var filter_category = Request.Form["filter_category"].FirstOrDefault();
            var filter_source = Request.Form["filter_source"].FirstOrDefault();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Laporan CAPA");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "#";
                worksheet.Cell(currentRow, 2).Value = "No. CAPA";
                worksheet.Cell(currentRow, 3).Value = "Sumber";
                worksheet.Cell(currentRow, 4).Value = "Masalah";
                worksheet.Cell(currentRow, 5).Value = "Tgl. Pengajuan";
                worksheet.Cell(currentRow, 6).Value = "Initiator";

                var dataCapa = context.capa.GroupJoin(
                        context.employee,
                        capa => capa.initiator,
                        employee => employee.id,
                        (capa, employee) => new { capa, employee }
                    )
                    .SelectMany(
                    x => x.employee.DefaultIfEmpty(),
                    (capa, employee) => new
                    {
                        capa_no = capa.capa.capa_no,
                        source = capa.capa.source,
                        problem = capa.capa.problem,
                        initiation_date = capa.capa.initiation_date,
                        initiator = capa.capa.initiator,
                        id_category = capa.capa.id_category,
                        initiator_name = employee.display_name
                    }
                );

                if (!string.IsNullOrEmpty(filter_initiator))
                {
                    dataCapa = dataCapa.Where(m => m.initiator == int.Parse(filter_initiator));
                }

                if (!string.IsNullOrEmpty(filter_initiation_date))
                {
                    string[] initiation_date = filter_initiation_date.Split(" - ");
                    DateTime initiation_from = DateTime.ParseExact(initiation_date[0], "dd/MM/yyyy", null);
                    DateTime initiation_to = DateTime.ParseExact(initiation_date[1], "dd/MM/yyyy", null);

                    dataCapa = dataCapa.Where(m => m.initiation_date >= initiation_from && m.initiation_date <= initiation_to);
                }

                if (!string.IsNullOrEmpty(filter_category))
                {
                    dataCapa = dataCapa.Where(m => m.id_category == int.Parse(filter_category));
                }

                if (!string.IsNullOrEmpty(filter_source))
                {
                    dataCapa = dataCapa.Where(m => m.source.Contains(filter_source));
                }

                foreach (var list in dataCapa)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = (currentRow - 1).ToString();
                    worksheet.Cell(currentRow, 2).Value = list.capa_no;
                    worksheet.Cell(currentRow, 3).Value = list.source;
                    worksheet.Cell(currentRow, 4).Value = list.problem;
                    worksheet.Cell(currentRow, 5).Value = list.initiation_date.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cell(currentRow, 6).Value = list.initiator_name;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Laporan CAPA.xlsx"
                    );
                }


            }
        }
    }
}
