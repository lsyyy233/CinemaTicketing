using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Entity
{
	public class Hall
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Seats { get; set; }
		public List<Show> Shows { get; set; }
	}
}
