using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Entity
{
	/// <summary>
	/// 电影票
	/// </summary>
	public class Ticket
	{
		public int Id { get; set; }
		public int SeatNum { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public int ShowId { get; set; }
		public Show Show { get; set; }
	}
}
