using AutoMapper;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Dtos.UpdateDtos;
using CinemaTicketing.Models.Entity;

namespace CinemaTicketing.Models.Profiles
{
	public class HallFrofile : Profile
	{
		public HallFrofile()
		{
			CreateMap<Hall, HallDto>();
			CreateMap<HallAddDto, Hall>();
			CreateMap<HallUpdateDto, Hall>();
		}
	}
}
