using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Helpers.Pagination
{
	public class PagedParametersBase
	{
		private const int MaxPageSize = 30;
		public int PageNumber { get; set; } = 1;
		private int pageSize = 5;
		public int PageSize
		{
			get => pageSize;
			set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
		}
	}
}
