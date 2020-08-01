using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos.DtoParameters
{
	public class MovieDtoParameters
	{
		private const int MaxPageSize = 30;
		public int? Id { get; set; }
		public string CompanyName { get; set; }
		public string SearchTerm { get; set; }
		public int PageNumber { get; set; } = 1;
		private int pageSize = 5;
		public string Fields { get; set; }

		public int PageSize
		{
			get => pageSize;
			set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
		}
	}
}
