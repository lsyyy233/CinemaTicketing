using AutoMapper;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Profiles
{
	public class LoggedUserProfile : Profile
	{
		public LoggedUserProfile()
		{
			CreateMap<LoggedUser, LoggedUserDto>();
		}
	}
}
