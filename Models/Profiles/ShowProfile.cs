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
	public class ShowProfile : Profile
	{
		public ShowProfile()
		{
			CreateMap<Show, ShowDto>()
				.ForMember(dest => dest.ShowNum, opt => opt.MapFrom(src => src.ShowNum.ToString()))
				.ForMember(dest =>dest.Expired,opt =>opt.MapFrom(src => (DateTime.Compare(DateTime.Now.Date,src.DateTime)) >0));
			CreateMap<ShowAddDto, Show>()
				.ForMember(dest => dest.ShowNum, opt => opt.MapFrom(src => Enum.Parse<ShowNum>(src.ShowNum)))
				.ForMember(dest =>dest.DateTime, opt => opt.MapFrom(src => src.DateTime.Date));
		}
	}
}
