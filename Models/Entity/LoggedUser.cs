using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Entity
{
	public class LoggedUser
	{
		//public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public Guid Guid { get; set; }
	}
}
