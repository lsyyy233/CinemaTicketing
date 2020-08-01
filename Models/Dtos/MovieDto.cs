using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos
{
	public enum ReleaseStatu
	{
		暂无场次 = 0,
		正在上映,
		已经下映
	}
	public class MovieDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Introduction { get; set; }
		public ReleaseStatu ReleaseStatu { get; set; }
	}
}
