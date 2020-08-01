using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos.UpdateDtos
{
	public class MovieUpdateDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Introduction { get; set; }
		public bool IsUnderTheHit { get; set; }
	}
}
