using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
	public class Capa
	{
		[Key]
		[Display(Name = "ID")]
		public int id { get; set; }

		[Required]
		//[Index(IsUnique = true)]
		public int capa_no { get; set; }

		public DateTime initiation_date { get; set; }

		[Required(ErrorMessage = "*Required Source")]
		[StringLength(255, ErrorMessage = "Source cannot be longer than 255 characters.")]
		public string source { get; set; }

		[Required(ErrorMessage = "*Required Problem")]
		public string problem { get; set; }

		[Required]
		public int initiator { get; set; }

		[Required(ErrorMessage = "*Required Admin")]
		public int id_admin { get; set; }

		public int? id_category { get; set; }

		[StringLength(255, ErrorMessage = "Is Proper cannot be longer than 150 characters.")]
		public string? is_proper { get; set; }

		[StringLength(50, ErrorMessage = "Status cannot be longer than 50 characters.")]
		public string? status { get; set; }

		public int flag { get; set; }

		public int is_active { get; set; }

		public Capa()
		{
			is_active = 1;
		}

	}
}
