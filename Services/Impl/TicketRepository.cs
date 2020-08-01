using CinemaTicketing.Models;
using CinemaTicketing.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services.Impl
{
	public class TicketRepository :  ITicketRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;
		public TicketRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext;
		}
		/// <summary>
		/// 售票
		/// </summary>
		/// <param name="hallId">影厅Id</param>
		/// <param name="seatNum">座位号</param>
		public void AddTicket(Ticket ticket)
		{
			if (ticket == null)
			{
				throw new ArgumentNullException(nameof(ticket));
			}
			try
			{
				ticket.Id = _DbContext.Tickets.Select(x => x.Id).Max() + 1;
			}
			catch (System.InvalidOperationException)
			{
				ticket.Id = 1;
			}
			_DbContext.Tickets.Add(ticket);
		}
		/// <summary>
		/// 退票
		/// </summary>
		/// <param name="ticketId"></param>
		public void DeleteTicket(Ticket ticket)
		{
			_DbContext.Tickets.Remove(ticket);
		}
		/// <summary>
		/// 获取电影票信息
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		public async Task<Ticket> GetTicketAsync(int ticketId)
		{
			return await _DbContext.Tickets
				.Where(x => x.Id == ticketId)
				.SingleOrDefaultAsync();
		}

		public async Task<bool> SaveAsync()
		{
			return await _DbContext.SaveChangesAsync() >= 0;
		}
		/// <summary>
		/// 检查该场次的指定座位是否已经售出
		/// </summary>
		/// <param name="showId"></param>
		/// <param name="seatNum"></param>
		/// <returns>如果已经售出，返回true，否则返回false</returns>
		public async Task<bool> SeatHasSaledAsync(int showId, int seatNum)
		{
			return await _DbContext.Tickets
				.Where(x => x.ShowId == showId && x.SeatNum == seatNum)
				.SingleOrDefaultAsync()
				!= null;
		}
	}
}
