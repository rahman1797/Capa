using CAPA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.ViewModel
{
    public class MyViewModel
    {
        public List<Capa> Capas { get; set; }
        public List<RootCause> RootCauses { get; set; }
        public List<RelatedWorkUnit> Related_Work_Units { get; set; }
        public List<Department> Departments { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Employee> InitiatorCapa { get; set; }
        public List<CorrectionAction> Correction_Actions { get; set; }
        public List<Verification> Verifications { get; set; }
        public List<EmailLog> Email_Logs { get; set; }
    }
}
