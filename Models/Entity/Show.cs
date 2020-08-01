using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Entity
{
	public enum ShowNum
	{
		第一场 = 1,
		第二场,
		第三场,
		第四场,
		第五场,
		第六场
	}
	/// <summary>
	/// 场次
	/// </summary>
	public class Show
	{
		public int Id { get; set; }
		public DateTime DateTime { get; set; }
		public ShowNum ShowNum { get; set; }
		public int MovieId { get; set; }
		public int HallId { get; set; }
		public Movie Movie { get; set; }
		public Hall Hall { get; set; }
		public double Price { get; set; }
		public List<Ticket> Tickets { get; set; }
	}
}
