using CinemaTicketing.Helpers.Pagination;
using CinemaTicketing.Models;
using CinemaTicketing.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CinemaTicketing.Services.Impl
{
	public class TicketRepository : ITicketRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;
		public TicketRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext ?? throw new ArgumentNullException(nameof(cinemaTicketingDbContext));
		}

		/// <summary>
		/// 获取指定用户购买的所有电影票
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<PagedListBase<Ticket>> GetTicketsForUserAsync(PagedParametersBase pagedParameters, [Optional] int? userId)
		{
			IQueryable<Ticket> ticketsQueryable = _DbContext.Tickets
				.AsQueryable<Ticket>();
			if (userId != null)
			{
				ticketsQueryable = ticketsQueryable.Where(x => x.UserId == userId);
			}
			PagedListBase<Ticket> pagedTicketsForUser = await PagedListBase<Ticket>.CreateAsync(
				ticketsQueryable,
				pagedParameters.PageNumber,
				pagedParameters.PageSize);
			return pagedTicketsForUser;
		}
		/// <summary>
		/// 获取指定场次未售出的座位号
		/// </summary>
		/// <param name="showId"></param>
		/// <returns></returns>
		public async Task<List<int>> GetSaledSeatListAsync(int showId)
		{
			List<int> saledSeatList = await _DbContext.Tickets
				.Where(x => x.ShowId == showId)
				.Select(x => x.SeatNum)
				.ToListAsync();
			return saledSeatList;
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
