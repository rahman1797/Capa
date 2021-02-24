using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class Department
    {
        [Key]
        [Display(Name = "ID")]
        public int? id { get; set; }

        [Required(ErrorMessage = "Department Name Required")]
        [MaxLength(255)]
        [Display(Name = "Department Name")]
        public string department_name { get; set; }

        [Display(Name = "Is Active")]
        public int is_active { get; set; }

        [Display(Name = "Created At")]
        public DateTime created_at { get; set; }

        [Display(Name = "Updated At")]
        public DateTime updated_at { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Updated By")]
        public string updated_by { get; set; }

        public Department()
        {
            is_active = 1; 
        }
    }
}
