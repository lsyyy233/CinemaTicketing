using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services
{
	public interface ITicketRepository
	{
		Task<bool> SeatHasSaledAsync(int showId, int seatNum);
		void AddTicket(Ticket ticket);
		void DeleteTicket(Ticket ticket);
		Task<Ticket> GetTicketAsync(int ticketId);
		/// <summary>
		/// 将修改应用到数据库
		/// </summary>
		/// <returns>是否执行成功</returns>
		Task<bool> SaveAsync();
	}
}
