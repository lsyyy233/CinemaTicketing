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
	public class TicketProfile : Profile
	{
		public TicketProfile()
		{
			CreateMap<Ticket, TicketDto>();
			CreateMap<TicketAddDto, Ticket>();
		}
	}
}
