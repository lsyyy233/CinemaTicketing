using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos
{
	public class UserDto
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public UserType UserType { get; set; }
	}
}
