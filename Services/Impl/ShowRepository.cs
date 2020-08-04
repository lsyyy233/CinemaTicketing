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
	public class ShowRepository : IShowRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;
		public ShowRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext;
		}
		/// <summary>
		/// 获取指定日期和影厅的有空闲的场次
		/// </summary>
		/// <param name="date"></param>
		/// <param name="hall"></param>
		/// <returns></returns>
		public async Task<List<string>> GetAvailableShowsAsync(DateTime date, int hallId, int? showId)
		{
			string[] showNums = Enum.GetNames(typeof(ShowNum));
			List<string> availableShows = new List<string>();
			availableShows.AddRange(showNums);
			List<ShowNum> lists = await _DbContext.Shows
				.Where(x => x.DateTime == date && x.HallId == hallId)
				.Select(x => x.ShowNum)
				.ToListAsync();
			foreach (ShowNum showNum in lists)
			{
				availableShows.Remove(showNum.ToString());
			}
			if (showId != null)
			{
				Show show = await _DbContext.Shows
					.Where(x => x.Id == showId && x.HallId == hallId)
					.SingleOrDefaultAsync();
				if (show != null)
				{
					availableShows.Add(show.ShowNum.ToString());
				}

			}
			return availableShows;
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
			if (show.Id == null)
			{
				try
				{
					show.Id = _DbContext.Shows.Select(x => x.Id).Max() + 1;
				}
				catch (System.InvalidOperationException)
				{
					show.Id = 1;
				}
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
		public async Task<PagedListBase<Show>> GetShowsAsync(PagedParametersBase pagedParameters)
		{
			IQueryable<Show> queryExpression = _DbContext.Shows.AsQueryable<Show>();
			PagedListBase<Show> pagedShows = await PagedListBase<Show>.CreateAsync(
				queryExpression,
				pagedParameters.PageNumber,
				pagedParameters.PageSize
				);
			return pagedShows;
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
			if (movieId != null)
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
			return await result.ToListAsync();
		}
	}
}
