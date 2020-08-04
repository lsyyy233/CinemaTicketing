using CinemaTicketing.Helpers.Pagination;
using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CinemaTicketing.Services
{
	public interface IShowRepository
	{
		void AddShow(Show show);
		void UpdateShow(Show show);
		void DeleteShow(Show show);
		Task<PagedListBase<Show>> GetShowsAsync(PagedParametersBase pagedParameters);
		Task<Show> GetShowAsync(int showId);
		Task<IEnumerable<Show>> GetShowsOfToday();
		Task<IEnumerable<Show>> GetShowsOfHallAtOneDay(int hallId, DateTime dateTime, ShowNum? showNum);
		/// <summary>
		/// 将修改应用到数据库
		/// </summary>
		/// <returns>是否执行成功</returns>
		Task<bool> SaveAsync();
		Task<List<Show>> GetShowsNotExpire(int? movieId);
		Task<List<string>> GetAvailableShowsAsync(DateTime date, int hallId,int? showId);
	}
}
