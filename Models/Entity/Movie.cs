using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Entity
{
	
	public class Movie
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Introduction { get; set; }
		public List<Show> Shows { get; set; }
		public bool IsUnderTheHit { get; set; }
	}
}
