using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class RelatedWorkUnit
    {
        [Key]
        public int id { get; set; }
        public int capa_no { get; set; }
        public int id_employee { get; set; }
        public string rule { get; set; }
        public int id_department { get; set; }        
    }
}
