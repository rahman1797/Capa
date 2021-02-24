using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class Employee
    {
        [Key]
        public int? id { get; set; }

        [Required(ErrorMessage = "Required DisplayName")]
        public string display_name { get; set; }


        [Required(ErrorMessage = "Required Email")]
        public string email { get; set; }

        public int is_active { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }



    }
}
