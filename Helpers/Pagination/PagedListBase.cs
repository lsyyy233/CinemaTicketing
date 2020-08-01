using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Helpers.Pagination
{
	public  class PagedListBase<T> : List<T>
	{
		public int CurrentPage { get; private set; }
		public int TotalPages { get; private set; }
		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }
		public bool HasPrevious => CurrentPage > 1;
		public bool HasNext => CurrentPage < TotalPages;
		public PagedListBase(List<T> items, int count, int pageSize, int currentPage)
		{
			TotalCount = count;
			PageSize = pageSize;
			CurrentPage = currentPage;
			TotalPages = (int)Math.Ceiling((double)count / (double)pageSize);
			AddRange(items);
		}
		public static async Task<PagedListBase<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
		{
			int count = await source.CountAsync();
			var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
			return new PagedListBase<T>(items, count, pageSize, pageNumber);
		}
	}
}
