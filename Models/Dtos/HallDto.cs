using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos
{
	public class HallDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Seats { get; set; }
	}
}
