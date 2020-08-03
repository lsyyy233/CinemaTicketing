using CinemaTicketing.Helpers.Pagination;
using CinemaTicketing.Models;
using CinemaTicketing.Models.Entity;
using Masuit.Tools.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services.Impl
{
	public class MovieRepository : IMovieRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;
		public MovieRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext;
		}
		/// <summary>
		/// 获取所有没有下映的电影
		/// </summary>
		/// <returns></returns>
		public async Task<List<Movie>> GetMoviesNotUnderTheHitAsync()
		{
			List<Movie> movies = await _DbContext.Movies
				.Where(x => x.IsUnderTheHit == false)
				.ToListAsync();
			return movies;
		}
		/// <summary>
		/// 添加电影到影库
		/// </summary>
		/// <param name="movie"></param>
		public void AddMovieAsync(Movie movie)
		{
			if (movie == null)
			{
				throw new ArgumentNullException(nameof(movie));
			}
			try
			{
				movie.Id = _DbContext.Movies.Select(x => x.Id).Max() + 1;
			}
			catch (System.InvalidOperationException)
			{
				movie.Id = 1;
			}
			_DbContext.Movies.Add(movie);
		}

		/// <summary>
		/// 从影库删除电影
		/// </summary>
		/// <param name="movie"></param>
		public void DeleteMovie(Movie movie)
		{
			if (movie == null)
			{
				throw new ArgumentNullException(nameof(movie));
			}
			_DbContext.Movies.Remove(movie);
		}
		/// <summary>
		/// 根据Id获取电影
		/// </summary>
		/// <param name="MovieId"></param>
		/// <returns></returns>
		public async Task<Movie> GetMovieAsync(int MovieId)
		{
			return await _DbContext.Movies
				.Where(x => x.Id == MovieId)
				.FirstOrDefaultAsync();
		}
		/// <summary>
		/// 获取所有电影
		/// </summary>
		/// <returns></returns>
		public async Task<PagedListBase<Movie>> GetMoviesAsync(PagedParametersBase pagedParameters)
		{
			IQueryable<Movie> queryExpression = _DbContext.Movies.AsQueryable<Movie>();
			PagedListBase<Movie> pagedMovies = await PagedListBase<Movie>.CreateAsync(
				queryExpression,
				pagedParameters.PageNumber,
				pagedParameters.PageSize
				);
			return pagedMovies;
		}

		public void UpdateMovie(Movie movie)
		{

		}
		public async Task<bool> SaveAsync()
		{
			return await _DbContext.SaveChangesAsync() >= 0;
		}
	}
}
