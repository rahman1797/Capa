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
    public class ListInProgressController : BaseController
    {
        private readonly CapaDbContext context;
        private readonly IEmailSender _emailSender;
        private string email, nameAdm, nameInit;
        public ListInProgressController(CapaDbContext context, IEmailSender emailSender)
        {
            this.context = context;
            this._emailSender = emailSender;
        }

        public IActionResult Index()
        {
            ViewData["pakai_datatables"] = "ya";
            return View();
        }

        public IActionResult GetData(int ID)
        {
            var capa_number = ID;

            /*ViewBag.Capa = context.Capa.GroupJoin(
                        context.Employee,
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
                        status = capa.capa.status,
                        initiator = capa.capa.initiator,
                        id_category = capa.capa.id_category,
                        initiator_name = employee.display_name
                    }
                );*/

            ViewBag.RC = (from db_rc in context.root_cause join db_employee in context.employee
                          on db_rc.updated_by equals db_employee.id
                          where db_rc.capa_no == capa_number
                          select new
                          {
                              capa_no = db_rc.capa_no,
                              root_cause = db_rc.root_cause,
                              created_at = db_rc.created_at,
                              updated_at = db_rc.updated_at,
                              updated_by = db_employee.display_name
                          }).ToList();

            var getCapa = (from db_capa in context.capa where db_capa.capa_no == capa_number select db_capa).ToList();
            var getRC = (from db_rc in context.root_cause where db_rc.capa_no == capa_number select db_rc).ToList();
            var getRWU = (from db_rwu in context.related_work_unit where db_rwu.capa_no == capa_number select db_rwu).ToList();
            var getDept = (from db_rwu in context.related_work_unit
                           join db_department in context.department on db_rwu.id_department equals db_department.id
                           where db_rwu.capa_no == capa_number
                           select db_department).ToList();
            var getEmp = (from db_rwu in context.related_work_unit
                          join db_employee in context.employee on db_rwu.id_employee equals db_employee.id
                          where db_rwu.capa_no == capa_number
                          select db_employee).ToList();
            var getCA = (from db_ca in context.correction_action
                         where db_ca.capa_no == capa_number
                         select db_ca).ToList();
            var getVerif = (from db_verification in context.verification where db_verification.capa_no == capa_number select db_verification).ToList();
            var getEmailLog = (from db_email_log in context.email_log where db_email_log.capa_no == capa_number select db_email_log).ToList();
            var getInitiator = (from db_capa in context.capa
                                join db_employee in context.employee on db_capa.initiator equals db_employee.id
                                where db_capa.capa_no == capa_number
                                select db_employee).ToList();

            MyViewModel models = new MyViewModel()
            {
                Capas = getCapa.ToList(),
                RootCauses = getRC.ToList(),
                Related_Work_Units = getRWU.ToList(),
                Departments = getDept.ToList(),
                Employees = getEmp.ToList(),
                Correction_Actions = getCA.ToList(),
                Verifications = getVerif.ToList(),
                Email_Logs = getEmailLog.ToList(),
                InitiatorCapa = getInitiator.ToList(),
            };

            ViewBag.LIP = capa_number;
            return View("GetData", models);
        }

        [HttpPost]
        public IActionResult GetList()
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

                var listCapa = context.capa.GroupJoin(
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
                    listCapa = listCapa.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    listCapa = listCapa.Where(m => m.source.Contains(searchValue)
                    || m.problem.Contains(searchValue));
                }

                recordsTotal = listCapa.Count();

                var data = listCapa.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
