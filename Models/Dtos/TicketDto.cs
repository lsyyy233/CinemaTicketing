using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos
{
	public class TicketDto
	{
		public int Id { get; set; }
		//public int UserId { get; set; }
		public UserDto UserDto { get; set; }
		//public int ShowId { get; set; }
		public ShowDto ShowDto { get; set; }
		public int SeatNum { get; set; }
	}
}
