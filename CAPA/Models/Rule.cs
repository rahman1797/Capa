using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using System.ComponentModel.DataAnnotations;

namespace CAPA.Models
{
    public class Rule
    {
        [Key]
        public int? id { get; set; }

        [Required(ErrorMessage = "Required DisplayName")]
        public string rule_name { get; set; } 
    }
}
