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
	public class ShowRepository : IShowRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;
		public ShowRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext;
		}

		/// <summary>
		/// 添加场次
		/// </summary>
		/// <param name="show"></param>
		public void AddShow(Show show)
		{
			if (show == null)
			{
				throw new ArgumentNullException(nameof(show));
			}
			try
			{
				show.Id = _DbContext.Shows.Select(x => x.Id).Max() + 1;
			}
			catch (System.InvalidOperationException)
			{
				show.Id = 1;
			}
			_DbContext.Shows.Add(show);
		}
		/// <summary>
		/// 删除场次
		/// </summary>
		/// <param name="show"></param>
		public void DeleteShow(Show show)
		{
			if (show == null)
			{
				throw new ArgumentNullException(nameof(show));
			}
			_DbContext.Shows.Remove(show);
		}
		/// <summary>
		/// 获取单个场次
		/// </summary>
		/// <param name="showId"></param>
		/// <returns></returns>
		public async Task<Show> GetShowAsync(int showId)
		{
			return await _DbContext.Shows
				   .Where(x => x.Id == showId)
				   .SingleOrDefaultAsync();
		}
		/// <summary>
		/// 获取所有场次
		/// </summary>
		/// <returns></returns>
		public async Task<List<Show>> GetShowsAsync()
		{
			return await _DbContext.Shows.ToListAsync();
		}
		/// <summary>
		/// 获取当天的所有场次
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<Show>> GetShowsOfToday()
		{
			DateTime today = DateTime.Today.Date;
			return await _DbContext.Shows
				.Where(x => x.DateTime == today)
				.ToListAsync();
			 
		}
		/// <summary>
		/// 获取指定电影Id的所有未过期的场次
		/// </summary>
		/// <param name="movieId">可空类型，如果为空返回所有未过期的场次</param>
		/// <returns>指定电影Id的所有未过期的场次或所有未过期的场次</returns>
		public async Task<List<Show>> GetShowsNotExpire(int? movieId)
		{

			IQueryable<Show> shows = _DbContext.Shows
			.Where(x => (DateTime.Compare(DateTime.Now.Date, x.DateTime)) <= 0);
			if(movieId != null)
			{
				shows = shows.Where(x => x.MovieId == movieId);
			}
			return await shows.ToListAsync();
		}
		/// <summary>
		/// 更新场次
		/// </summary>
		/// <param name="show"></param>
		public void UpdateShow(Show show)
		{
		}
		public async Task<bool> SaveAsync()
		{
			return await _DbContext.SaveChangesAsync() >= 0;
		}

		public async Task<IEnumerable<Show>> GetShowsOfHallAtOneDay(int hallId, DateTime dateTime, ShowNum? showNum)
		{
			IQueryable<Show> result = _DbContext.Shows
				.Where(x => (x.HallId == hallId) && x.DateTime == dateTime);
			if (showNum != null)
			{
				result = result.Where(x => x.ShowNum == showNum);
			}
			return await result.ToListAsync() ;
		}
	}
}
