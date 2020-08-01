using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos.AddDtos
{
	public class ShowAddDto
	{
		public DateTime DateTime { get; set; }
		public string ShowNum { get; set; }
		public int MovieId { get; set; }
		public int HallId { get; set; }
		public double Price { get; set; }
	}
}
