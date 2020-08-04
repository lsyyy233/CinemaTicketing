using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos.AddDtos
{
	public class TicketAddDto
	{
		public Guid Guid { get; set; }
		public int ShowId { get; set; }
		public int SeatNum { get; set; }

	}
}
