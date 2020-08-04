using AutoMapper;
using CinemaTicketing.Helpers;
using CinemaTicketing.Helpers.Pagination;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinemaTicketing.Controllers
{
	[ApiController]
	[Route("api/tickets")]
	public class TicketController : ControllerBase
	{
		private readonly IShowRepository showRepository;
		private readonly IUserRepository userRepository;
		private readonly ITicketRepository ticketRepository;
		private readonly ILoggedUserRepository loggedUserRepository;
		private readonly IHallRepository hallRepository;
		private readonly IMovieRepository movieRepository;
		private readonly IAuthentication authentication;
		private readonly IMapper mapper;
		public TicketController(
			ITicketRepository ticketRepository,
			IMapper mapper,
			IShowRepository showRepository,
			IUserRepository userRepository,
			ILoggedUserRepository loggedUserRepository,
			IHallRepository hallRepository,
			IMovieRepository movieRepository,
			IAuthentication authentication)
		{
			this.ticketRepository = ticketRepository;
			this.mapper = mapper;
			this.showRepository = showRepository;
			this.userRepository = userRepository;
			this.loggedUserRepository = loggedUserRepository;
			this.hallRepository = hallRepository;
			this.movieRepository = movieRepository;
			this.authentication = authentication;
		}

		/// <summary>
		/// 获取用户购买的所有电影票
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		[HttpGet("{guid}", Name = nameof(GetTicketsAsync))]
		public async Task<ActionResult> GetTicketsAsync(
			Guid guid,
			[FromQuery] PagedParametersBase pagedParameters,
			[FromHeader(Name = "Accept")] string mediaType)
		{
			User user = await authentication.GetUserTypeAsync(guid);
			if (user == null)
			{
				return Unauthorized();
			}
			if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
			{
				return BadRequest();
			}
			PagedListBase<Ticket> pagedickets;
			//如果Guid对应的账户为管理员，返回所有电影票
			if (user.UserType == UserType.Administrator)
			{
				pagedickets = await ticketRepository.GetTicketsForUserAsync(pagedParameters);
			}
			//否则返回登录的用户的电影票
			else
			{
				pagedickets = await ticketRepository.GetTicketsForUserAsync(pagedParameters, user.Id);
			}
			//转换
			List<TicketDto> pagedTicketDtos = new List<TicketDto>();
			foreach (Ticket ticket in pagedickets)
			{
				TicketDto ticketDto = mapper.Map<TicketDto>(ticket);
				//如果登录的是管理员，在返回的电影票集合中附加对应的用户信息
				if (user.UserType == UserType.Administrator)
				{
					User userOfTicket = await userRepository.GetUserAsync(ticket.UserId);
					ticketDto.UserDto = mapper.Map<UserDto>(userOfTicket);
				}
				Show show = await showRepository.GetShowAsync(ticket.ShowId);
				show.Movie = await movieRepository.GetMovieAsync(show.MovieId);
				show.Hall = await hallRepository.GetHallAsync(show.HallId);
				ticketDto.ShowDto = mapper.Map<ShowDto>(show);
				pagedTicketDtos.Add(ticketDto);
			}
			if (parsedMediaType.MediaType == "application/vnd.cinemaTicketing.hateoas+json")
			{
				List<dynamic> linkedTicketDtos = new List<dynamic>();
				foreach (TicketDto data in pagedTicketDtos)
				{
					IEnumerable<LinkDto> linksForTicket = CreateLinksForHall(data.Id);
					linkedTicketDtos.Add(new { data, linksForTicket });
				}
				//添加翻页信息
				IEnumerable<LinkDto> linksForShows = CreateLinksForHalls(pagedParameters, pagedickets.HasPrevious, pagedickets.HasNext, user.UserType, guid);
				var result = new { linkedTicketDtos, linksForShows, pagedickets.TotalCount, pagedickets.CurrentPage, pagedickets.TotalPages };
				return Ok(result);
			}
			return Ok(pagedTicketDtos);
		}

		/// <summary>
		/// 获取指定场次未售出的座位号
		/// </summary>
		/// <param name="showId"></param>
		/// <returns></returns>
		[HttpGet("{showId}/seats")]
		public async Task<ActionResult> GetSeats(int showId)
		{
			Show show = await showRepository.GetShowAsync(showId);
			if (show == null)
			{
				return NotFound();
			}
			Hall hall = await hallRepository.GetHallAsync(show.HallId);
			int seats = hall.Seats;
			List<int> seatList = new List<int>();
			for (int i = 1; i <= seats; i++)
			{
				seatList.Add(i);
			}
			List<int> saledSeatList = await ticketRepository.GetSaledSeatListAsync(showId);
			seatList.RemoveAll(x => saledSeatList.Contains(x));
			return Ok(seatList);
		}
		/// <summary>
		/// 获取电影票信息
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		[HttpGet(Name = nameof(GetTicketInfoAsync))]
		public async Task<ActionResult<Ticket>> GetTicketInfoAsync(int ticketId)
		{
			Ticket ticket = await ticketRepository.GetTicketAsync(ticketId);
			if (ticket == null)
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
		[HttpPost(Name = nameof(SaleTicketAsync))]
		public async Task<ActionResult<TicketDto>> SaleTicketAsync([FromBody] TicketAddDto ticketAddDto)
		{
			//检查对应的场次和用户是否存在
			Show show = await showRepository.GetShowAsync(ticketAddDto.ShowId);
			if (show == null)
			{
				return NotFound();
			}
			User user = await authentication.GetUserTypeAsync(ticketAddDto.Guid);
			if (user == null || user.UserType != UserType.RegularUser)
			{
				return Unauthorized();
			}
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
		/// <summary>
		/// 删除电影票（退票）
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		[HttpDelete(Name = nameof(DeleteTicket))]
		public async Task<ActionResult> DeleteTicket(Guid guid, int ticketId)
		{
			User user = await authentication.GetUserTypeAsync(guid);
			if (user == null || user.UserType != UserType.Administrator)
			{
				return Unauthorized();
			}
			Ticket ticket = await ticketRepository.GetTicketAsync(ticketId);
			if (ticket == null)
			{
				return NotFound();
			}
			ticketRepository.DeleteTicket(ticket);
			await ticketRepository.SaveAsync();
			return NoContent();
		}
		/// <summary>
		/// 为ticket添加链接
		/// </summary>
		/// <param name="showId"></param>
		/// <returns></returns>
		private IEnumerable<LinkDto> CreateLinksForHall(int showId)
		{
			List<LinkDto> links = new List<LinkDto>
			{
				new LinkDto(
					Url.Link(nameof(DeleteTicket), new { showId }),
					"delete ticket",
					"Delete")
			};
			return links;
		}
		/// <summary>
		/// 为show集合添加分页信息
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="hasPrevious"></param>
		/// <param name="hasNext"></param>
		/// <returns></returns>
		private IEnumerable<LinkDto> CreateLinksForHalls(PagedParametersBase parameters, bool hasPrevious, bool hasNext, UserType userType, Guid guid)
		{
			List<LinkDto> links = new List<LinkDto>
			{
				new LinkDto(CreateHallsResourceUri(parameters, ResourceUriType.CurrentPage, userType, guid),
				"self",
				"Get")
			};
			if (hasPrevious)
			{
				links.Add(new LinkDto(CreateHallsResourceUri(parameters, ResourceUriType.PreviousPage, userType, guid),
					"previous_page",
					"Get"));
			}
			if (hasNext)
			{
				links.Add(new LinkDto(CreateHallsResourceUri(parameters, ResourceUriType.NextPage, userType, guid),
					"next_page",
					"Get"));
			}
			return links;
		}
		/// <summary>
		/// 创建翻页链接
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="resourceUriType"></param>
		/// <returns></returns>
		private string CreateHallsResourceUri(PagedParametersBase parameters, ResourceUriType resourceUriType, UserType userType, Guid guid)
		{
			int pageNumber = 0;
			switch (resourceUriType)
			{
				case ResourceUriType.PreviousPage:
					{
						pageNumber = parameters.PageNumber - 1;
						break;
					}
				case ResourceUriType.NextPage:
					{
						pageNumber = parameters.PageNumber + 1;
						break;
					}
				case ResourceUriType.CurrentPage:
					{
						pageNumber = parameters.PageNumber;
						break;
					}
			}

			return Url.Link(nameof(GetTicketsAsync), new
			{
				guid,
				pageNumber,
				pageSize = parameters.PageSize
			});

		}
	}
}
