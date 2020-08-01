using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos
{
	public class LoggedUserDto
	{
		public UserDto UserDto { get; set; }
		public Guid Guid { get; set; }
	}
}
