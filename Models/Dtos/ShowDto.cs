using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos
{
	public class ShowDto
	{
		public int Id { get; set; }
		public DateTime DateTime { get; set; }
		public string ShowNum { get; set; }
		public int MovieId { get; set; }
		public int HallId { get; set; }
		public bool Expired { get; set; }
		public double Price { get; set; }
	}
}
