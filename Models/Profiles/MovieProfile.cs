using AutoMapper;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Dtos.UpdateDtos;
using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Profiles
{
	public class MovieProfile : Profile
	{
		public MovieProfile()
		{
			CreateMap<Movie, MovieDto>();
			CreateMap<MovieAddDto, Movie > ();
			CreateMap<MovieUpdateDto, Movie>();
		}
	}
}
