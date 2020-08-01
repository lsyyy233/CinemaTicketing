using CinemaTicketing.Models;
using CinemaTicketing.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services.Impl
{
	public class HallRepository : IHallRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;
		public HallRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext;
		}

		/// <summary>
		/// 添加影厅
		/// </summary>
		/// <param name="hall"></param>
		public void AddHall(Hall hall)
		{
			if (hall == null)
			{
				throw new ArgumentNullException(nameof(hall));
			}
			try
			{
				hall.Id = _DbContext.Halls.Select(x => x.Id).Max() + 1;
			}
			catch (System.InvalidOperationException)
			{
				hall.Id = 1;
			}
			_DbContext.Halls.Add(hall);
		}
		/// <summary>
		/// 获取影厅
		/// </summary>
		/// <param name="hallId"></param>
		/// <returns></returns>
		public async Task<Hall> GetHall(int hallId)
		{
			return await _DbContext.Halls
				.Where(x => x.Id == hallId)
				.FirstOrDefaultAsync();
		}
		public async Task<bool> SaveAsync()
		{
			return await _DbContext.SaveChangesAsync() >= 0;
		}
	}
}
