using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class CorrectionAction
    {
        [Key]
        public int id { get; set; }
        public int capa_no { get; set; }
        public string correction { get; set; }
        public string pic { get; set; }
        public DateTime deadline { get; set; }
        public DateTime realization { get; set; }
        public string type_correction { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int updated_by { get; set; }
    }
}
