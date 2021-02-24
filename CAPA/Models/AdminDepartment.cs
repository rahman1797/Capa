using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class AdminDepartment
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Department cannot be null")]
        public int id_department { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Employee cannot be null")]
        public int id_employee { get; set; }
        public int is_active { get; set; }

        /*[Required]*/
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy HH:mm}")]
        public DateTime created_at { get; set; }
        /*[Required]*/
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy HH:mm}")]
        public DateTime updated_at { get; set; }
        [Required]
        public string updated_by { get; set; }
    }
}
