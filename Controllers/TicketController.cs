using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicketing.Services;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Entity;
using AutoMapper;
using CinemaTicketing.Models.Dtos.AddDtos;

namespace CinemaTicketing.Controllers
{
	[ApiController]
	[Route("api/ticket")]
	public class TicketController :ControllerBase
	{
		private readonly IShowRepository showRepository;
		private readonly IUserRepository userRepository;
		private readonly ITicketRepository ticketRepository;
		private readonly IMapper mapper;
		public TicketController(ITicketRepository ticketRepository, IMapper mapper, IShowRepository showRepository, IUserRepository userRepository)
		{
			this.ticketRepository = ticketRepository;
			this.mapper = mapper;
			this.showRepository = showRepository;
			this.userRepository = userRepository;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		[HttpGet(Name =nameof(GetTicketInfoAsync))]
		public async Task<ActionResult<Ticket>> GetTicketInfoAsync(int ticketId)
		{
			Ticket ticket =await ticketRepository.GetTicketAsync(ticketId);
			if(ticket == null)
			{
				return NotFound();
			}
			TicketDto ticketDto = mapper.Map<TicketDto>(ticket);
			return Ok(ticketDto);
		}
		/// <summary>
		/// 售票（添加电影票）
		/// </summary>
		/// <returns></returns>
		[HttpPost(Name =nameof(SaleTicketAsync))]
		public async Task<ActionResult<TicketDto>> SaleTicketAsync([FromBody] TicketAddDto ticketAddDto)
		{
			//检查对应的场次和用户是否存在
			Show show = await showRepository.GetShowAsync(ticketAddDto.ShowId);
			User user = await userRepository.GetUserAsync(ticketAddDto.UserId);
			if(show == null || user == null)
			{
				return NotFound();
			}
			//检查该场次座位号是否已经售出
			bool seatHasSaled = await ticketRepository.SeatHasSaledAsync(ticketAddDto.ShowId, ticketAddDto.SeatNum);
			if (seatHasSaled)
			{
				return UnprocessableEntity();
			}
			//添加到数据库
			Ticket ticket = mapper.Map<Ticket>(ticketAddDto);
			ticketRepository.AddTicket(ticket);
			await ticketRepository.SaveAsync();
			//将用户信息和场次信息添加到返回的数据中
			ShowDto showDto = mapper.Map<ShowDto>(show);
			UserDto userDto = mapper.Map<UserDto>(user);
			TicketDto ticketDto = mapper.Map<TicketDto>(ticket);
			ticketDto.ShowDto = showDto;
			ticketDto.UserDto = userDto;
			return CreatedAtRoute(
				nameof(GetTicketInfoAsync), 
				new { ticketId = ticket.Id }, 
				ticketDto);
		}
	}
}
