using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class Category
    {
        [Key]
        public int? id { get; set; }

        [Required]
        public string category_name { get; set; }

        [Required]
        public string description { get; set; }

        [Required]
        public int is_active { get; set; }

        public DateTime created_at { get; set; }

        /*[Required]
        public string created_by { get; set; }*/

        public DateTime? updated_at { get; set; }

        public string updated_by { get; set; }
    }
}
