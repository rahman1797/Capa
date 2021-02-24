using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class EmailLog
    {
        [Key]
        public int id { get; set; }
        public int capa_no { get; set; }
        public string email_to { get; set; }
        public string email_from { get; set; }
        public string contents { get; set; }
        public DateTime created_at { get; set; }
        public int updated_by { get; set; }
    }
}
