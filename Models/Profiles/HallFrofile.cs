using AutoMapper;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Profiles
{
	public class HallFrofile : Profile
	{
		public HallFrofile()
		{
			CreateMap<Hall, HallDto>();
			CreateMap<HallAddDto, Hall>();
		}
	}
}
