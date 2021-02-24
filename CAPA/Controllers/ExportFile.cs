using CAPA.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace CAPA.Controllers
{
    public class ExportFile : Controller
    {
        private readonly CapaDbContext context;
        public ExportFile(CapaDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public string EmpName (int id)
        {
            var Name = (from Emp in context.employee where Emp.id == id select Emp.display_name);
            
            return Name.First();
        }
        public string DepartName(int id)
        {
            var Name = (from Dep in context.department where Dep.id == id select Dep.department_name);

            return Name.First();
        }
        public IActionResult ToExcel(int NoCapa)
        {

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Capa No " + NoCapa); //Nama dari worksheet

                //========================Set header Dari CAPA====================================================================
                sheet.Cell(1, 1).Value = "CAPA No ";
                sheet.Cell(2, 1).Value = "Source";
                sheet.Cell(3, 1).Value = "Problem";
                sheet.Cell(4, 1).Value = "Initiation date";
                sheet.Cell(5, 1).Value = "Initiator";
                sheet.Cell(6, 1).Value = "Status";
                sheet.Range(sheet.Cell(1, 1), sheet.Cell(6, 1)).Style.Font.Bold = true;
                sheet.Range(sheet.Cell(1, 2), sheet.Cell(6, 2)).Value = ":";

                //Isi dari Capa
                foreach (var list in context.capa.Where(val => val.capa_no == NoCapa))
                {
                    /*var EmpName = (from Emp in context.Employee where Emp.id == list.initiator select Emp.display_name).First();*/
                    sheet.Cell(1, 3).Value = list.capa_no;
                    sheet.Cell(2, 3).Value = list.source;
                    sheet.Cell(3, 3).Value = list.problem;
                    sheet.Cell(4, 3).Value = list.initiation_date;
                    sheet.Cell(4, 3).Style.NumberFormat.Format = "dd/mm/yyyy";

                    sheet.Cell(5, 3).Value = EmpName(list.initiator);
                    sheet.Cell(6, 3).Value = list.status;
                }
                sheet.Range(sheet.Cell(1, 1), sheet.Cell(6, 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                //========================Set header Dari Related Work Unit====================================================================
                sheet.Range(sheet.Cell(9, 1), sheet.Cell(9, 5)).Merge().Value = "Related Work Unit";
                sheet.Range(sheet.Cell(10, 1), sheet.Cell(10, 2)).Merge().Value = "DIVISION";
                sheet.Cell(10, 3).Value = "FILLER";
                sheet.Cell(10, 4).Value = "FOLLOW_UP";
                sheet.Cell(10, 5).Value = "APPROVAL";
                sheet.Range(sheet.Cell(10, 1), sheet.Cell(10, 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Range(sheet.Cell(9, 1), sheet.Cell(10, 5)).Style.Font.Bold = true;


                var currentRow = 10;
                //Isi dari Related Work Unit (RWU = Related Work Unit)
                foreach (var list_RWU in context.related_work_unit.Where(val => val.capa_no == NoCapa))
                {
                    currentRow++;
                    //DIVISION
                    sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 2)).Merge().Value = DepartName(list_RWU.id_department);

                    if (list_RWU.rule == "FILLER")
                    {
                        //FILLER
                        sheet.Cell(currentRow, 3).Value = EmpName(list_RWU.id_employee);
                        currentRow--;
                    }
                    if (list_RWU.rule == "FOLLOW UP")
                    {
                        //FOLLOW_UP
                        sheet.Cell(currentRow, 4).Value = EmpName(list_RWU.id_employee);
                        currentRow--;
                    }
                    if (list_RWU.rule == "APPROVAL")
                    {
                        //APPROVAL
                        sheet.Cell(currentRow, 5).Value = EmpName(list_RWU.id_employee);
                    }

                }
                sheet.Range(sheet.Cell(10, 1), sheet.Cell(currentRow, 5)).Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                sheet.Range(sheet.Cell(10, 1), sheet.Cell(currentRow, 5)).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                //========================Set header Dari Correction====================================================================
                currentRow += 3;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 8)).Merge().Value = "Correction";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 4)).Merge().Value = "Action (s)";
                sheet.Cell(currentRow + 1, 5).Value = "PIC";
                sheet.Cell(currentRow + 1, 6).Value = "Deadline";
                sheet.Cell(currentRow + 1, 7).Value = "Realization";
                sheet.Cell(currentRow + 1, 8).Value = "Updated By";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 8)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow + 1, 8)).Style.Font.Bold = true;

                currentRow++;
                var start_border = currentRow;
                //Isi dari Correction where type = "correction"
                foreach (var list_Correct in context.correction_action.Where(val => val.type_correction == "correction" && val.capa_no == NoCapa))
                {
                    currentRow++;
                    sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 4)).Merge().Value = list_Correct.correction;
                    sheet.Cell(currentRow, 5).Value = list_Correct.pic;
                    sheet.Cell(currentRow, 6).Value = list_Correct.deadline;
                    sheet.Cell(currentRow, 7).Value = list_Correct.realization;
                    sheet.Range(sheet.Cell(currentRow, 6), sheet.Cell(currentRow, 7)).Style.NumberFormat.Format = "dd/mm/yyyy";
                    sheet.Range(sheet.Cell(currentRow, 6), sheet.Cell(currentRow, 7)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    sheet.Cell(currentRow, 8).Value = EmpName(list_Correct.updated_by);
                }
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                //========================Set header Dari Root Cause====================================================================
                currentRow += 4;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 8)).Merge().Value = "Root Cause Identification";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 7)).Merge().Value = "Root Cause";
                sheet.Cell(currentRow + 1, 8).Value = "Updated By";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 8)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow + 1, 8)).Style.Font.Bold = true;

                currentRow++;
                start_border = currentRow;
                //Isi dari Root Cause where type = "correction"
                foreach (var list_RC in context.root_cause.Where(val => val.capa_no == NoCapa))
                {
                    currentRow++;
                    sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 7)).Merge().Value = list_RC.root_cause;
                    sheet.Cell(currentRow, 8).Value = (from Emp in context.employee where Emp.id == list_RC.updated_by select Emp.display_name).First(); ;
                }
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                //========================Set header Dari Corrective preventive====================================================================
                currentRow += 3;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 8)).Merge().Value = "Corrective/Preventive Action";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 4)).Merge().Value = "Action (s)";
                sheet.Cell(currentRow + 1, 5).Value = "PIC";
                sheet.Cell(currentRow + 1, 6).Value = "Deadline";
                sheet.Cell(currentRow + 1, 7).Value = "Realization";
                sheet.Cell(currentRow + 1, 8).Value = "Updated By";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 8)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow + 1, 8)).Style.Font.Bold = true;

                currentRow++;
                start_border = currentRow;
                //Isi dari Correction where type = "corrective_preventive"
                foreach (var list_Correct in context.correction_action.Where(val => val.type_correction == "corrective_preventive" && val.capa_no == NoCapa))
                {
                    currentRow++;
                    sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 4)).Merge().Value = list_Correct.correction;
                    sheet.Cell(currentRow, 5).Value = list_Correct.pic;
                    sheet.Cell(currentRow, 6).Value = list_Correct.deadline;
                    sheet.Cell(currentRow, 7).Value = list_Correct.realization;
                    sheet.Range(sheet.Cell(currentRow, 6), sheet.Cell(currentRow, 7)).Style.NumberFormat.Format = "dd/mm/yyyy";
                    sheet.Range(sheet.Cell(currentRow, 6), sheet.Cell(currentRow, 7)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    sheet.Cell(currentRow, 8).Value = EmpName(list_Correct.updated_by);
                }
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;



                //========================Set header Dari Verification====================================================================
                currentRow += 3;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 8)).Merge().Value = "Verification";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 7)).Merge().Value = "Result";
                sheet.Cell(currentRow + 1, 8).Value = "Updated By";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 8)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow + 1, 8)).Style.Font.Bold = true;

                currentRow++;
                start_border = currentRow;
                //Isi dari Verification
                foreach (var list_Verif in context.verification.Where(val => val.capa_no == NoCapa))
                {
                    currentRow++;
                    sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 7)).Merge().Value = list_Verif.verification;
                    sheet.Cell(currentRow, 8).Value = EmpName(list_Verif.updated_by);
                }
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                //========================Set header Dari Log Email====================================================================
                currentRow += 3;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow, 8)).Merge().Value = "Log Message";
                sheet.Cell(currentRow + 1, 1).Value = "Date";
                sheet.Range(sheet.Cell(currentRow + 1, 2), sheet.Cell(currentRow + 1, 3)).Merge().Value = "From";
                sheet.Range(sheet.Cell(currentRow + 1, 4), sheet.Cell(currentRow + 1, 5)).Merge().Value = "To";
                sheet.Range(sheet.Cell(currentRow + 1, 6), sheet.Cell(currentRow + 1, 8)).Merge().Value = "Message";
                sheet.Range(sheet.Cell(currentRow + 1, 1), sheet.Cell(currentRow + 1, 8)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Range(sheet.Cell(currentRow, 1), sheet.Cell(currentRow + 1, 8)).Style.Font.Bold = true;

                currentRow++;
                start_border = currentRow;
                //Isi dari Log Message
                foreach (var list_Log in context.email_log.Where(val => val.capa_no == NoCapa))
                {
                    currentRow++;
                    sheet.Cell(currentRow, 1).Value = list_Log.created_at;
                    sheet.Cell(currentRow, 1).Style.NumberFormat.Format = "dd/mm/yyyy";
                    sheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    sheet.Range(sheet.Cell(currentRow, 2), sheet.Cell(currentRow, 3)).Merge().Value = list_Log.email_from;
                    sheet.Range(sheet.Cell(currentRow, 4), sheet.Cell(currentRow, 5)).Merge().Value = list_Log.email_to;
                    sheet.Range(sheet.Cell(currentRow, 6), sheet.Cell(currentRow, 8)).Merge().Value = list_Log.contents;
                }
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                sheet.Range(sheet.Cell(start_border, 1), sheet.Cell(currentRow, 8)).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                sheet.Columns().AdjustToContents();  // Adjust column width
                sheet.Rows().AdjustToContents();     // Adjust row heights

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    //Save file
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Capa.xlsx");

                }
            }

        }
    }
}
