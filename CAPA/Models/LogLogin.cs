using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class LogLogin
    {
        public int id { get; set; }
        public int id_employee{ get; set; }
        public string rule { get; set; }
        public DateTime? last_login { get; set; }
        public string alamat_ip { get; set; }
        public string browser { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
