using CinemaTicketing.Helpers.Pagination;
using CinemaTicketing.Models.Entity;
using Masuit.Tools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services
{
	public interface IMovieRepository
	{
		Task<PagedListBase<Movie>> GetMoviesAsync(PagedParametersBase pagedParameters);
		Task<Movie> GetMovieAsync(int MovieId);
		void AddMovieAsync(Movie movie);
		void UpdateMovie(Movie movie);
		void DeleteMovie(Movie movie);
		/// <summary>
		/// 将修改应用到数据库
		/// </summary>
		/// <returns>是否执行成功</returns>
		Task<bool> SaveAsync();
		Task<List<Movie>> GetMoviesNotUnderTheHitAsync();
	}
}
