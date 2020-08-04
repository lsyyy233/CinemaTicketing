using AutoMapper;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinemaTicketing.Controllers
{
	[ApiController]
	[Route("api/tickets")]
	public class TicketController :ControllerBase
	{
		private readonly IShowRepository showRepository;
		private readonly IUserRepository userRepository;
		private readonly ITicketRepository ticketRepository;
		private readonly ILoggedUserRepository loggedUserRepository;
		private readonly IHallRepository hallRepository;
		private readonly IMovieRepository movieRepository;
		private readonly IMapper mapper;
		public TicketController(
			ITicketRepository ticketRepository,
			IMapper mapper,
			IShowRepository showRepository,
			IUserRepository userRepository,
			ILoggedUserRepository loggedUserRepository,
			IHallRepository hallRepository, 
			IMovieRepository movieRepository)
		{
			this.ticketRepository = ticketRepository;
			this.mapper = mapper;
			this.showRepository = showRepository;
			this.userRepository = userRepository;
			this.loggedUserRepository = loggedUserRepository;
			this.hallRepository = hallRepository;
			this.movieRepository = movieRepository;
		}
		[HttpGet("{showId}/seats")]
		public async Task<ActionResult> GetSeats(int showId)
		{
			Show show = await showRepository.GetShowAsync(showId);
			if(show == null)
			{
				return NotFound();
			}
			Hall hall = await hallRepository.GetHallAsync(show.HallId);
			int seats = hall.Seats;
			List<int> seatList = new List<int>();
			for(int i = 1;i<= seats; i++)
			{
				seatList.Add(i);
			}
			List<int> saledSeatList = await ticketRepository.GetSaledSeatListAsync(showId);
			seatList.RemoveAll( x => saledSeatList.Contains(x));
			return Ok(seatList);
		}
		/// <summary>
		/// 获取电影票信息
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
			Show show = await showRepository.GetShowAsync(ticket.ShowId);
			Movie movie = await movieRepository.GetMovieAsync(show.MovieId);
			Hall hall = await hallRepository.GetHallAsync(show.HallId);
			show.Movie = movie;
			show.Hall = hall;
			TicketDto ticketDto = mapper.Map<TicketDto>(ticket);
			ticketDto.ShowDto = mapper.Map<ShowDto>(show);
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
			LoggedUser loggedUser = await loggedUserRepository.GetLoggedUserAsync(ticketAddDto.Guid);
			if(show == null || loggedUser == null)
			{
				return NotFound();
			}
			User user = await userRepository.GetUserAsync(loggedUser.UserId);
			//检查该场次座位号是否已经售出
			bool seatHasSaled = await ticketRepository.SeatHasSaledAsync(ticketAddDto.ShowId, ticketAddDto.SeatNum);
			if (seatHasSaled)
			{
				return UnprocessableEntity();
			}
			//添加到数据库
			Ticket ticket = mapper.Map<Ticket>(ticketAddDto);
			ticket.UserId = user.Id;
			ticketRepository.AddTicket(ticket);
			await ticketRepository.SaveAsync();
			//将用户信息和场次信息添加到返回的数据中
			show.Movie = await movieRepository.GetMovieAsync(show.MovieId);
			show.Hall = await hallRepository.GetHallAsync(show.HallId);
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
