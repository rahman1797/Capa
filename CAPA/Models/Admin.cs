using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{

    public class Admin
    {
        [Key]
        [Display(Name = "ID")]
        public int id { get; set; }

        [ForeignKey("id_employee")]
        [Required(ErrorMessage = "*Required Admin")]
        public int id_employee { get; set; }

        [ForeignKey("id_rule")]
        [Required(ErrorMessage = "*Required Rule")]
        public int id_rule { get; set; }


        //[Required(ErrorMessage = "*Required Super Admin")]

        //[ForeignKey("id_employee_spv")]
        //[RequiredIf(nameof(id_rule), 1, ErrorMessage = "*Required Super Admin")]
        //[RequiredIf(nameof(id_rule), (Operator)ComparisonType.Equal, 1)] 
        // [RequiredIf(nameof(id_rule), 1)] 
        //[AssertThat("id_rule == 1 && id_employee_spv == 0", ErrorMessage = "*Required Super Admin")]
        //[DisplayName("Employee Supervisor")]  
        [Required(ErrorMessage = "*Required Super Admin")]
        public int? id_employee_spv { get; set; }

        [Required(ErrorMessage = "*Required Description")]
        [Column(TypeName = "nvarchar(MAX)")]
        public string description { get; set; }

        [Display(Name = "Is Active")]
        public int is_active { get; set; }

        [Required] 
        [Display(Name = "Created At")]
        public DateTime created_at { get; set; }

        [Display(Name = "Updated At")]
        public DateTime? updated_at { get; set; }

        [StringLength(255, ErrorMessage = "First name cannot be longer than 255 characters.")]
        public string updated_by { get; set; }

        //public Employee Employee { get; set; }
        // public Rule Rule { get; set; }

    }
}
