using AutoMapper;
using CinemaTicketing.Helpers;
using CinemaTicketing.Helpers.Pagination;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Dtos.UpdateDtos;
using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Controllers
{
	[Route("api/shows/")]
	[ApiController]
	public class ShowController : ControllerBase
	{
		private readonly IShowRepository showRepository;
		private readonly IAuthentication authentication;
		private readonly IMovieRepository movieRepository;
		private readonly IHallRepository hallRepository;
		private readonly IMapper mapper;
		public ShowController(
			IMapper mapper,
			IShowRepository repository,
			IMovieRepository movieRepository,
			IHallRepository hallRepository,
			IAuthentication authentication)
		{
			this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			this.showRepository = repository ?? throw new ArgumentNullException(nameof(showRepository));
			this.movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
			this.hallRepository = hallRepository ?? throw new ArgumentNullException(nameof(hallRepository));
			this.authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
		}
		/// <summary>
		/// 更新Show
		/// </summary>
		/// <param name="showUpdateDto"></param>
		/// <param name="guid"></param>
		/// <returns></returns>
		[HttpPut(Name = nameof(UpdateShow))]
		public async Task<ActionResult> UpdateShow(
			[FromBody] ShowUpdateDto showUpdateDto,
			[FromHeader] Guid guid)
		{
			//检查用户是否已经登录
			if (guid == Guid.Empty)
			{
				return Unauthorized();
			}
			//检查登陆的用户是否为管理员
			User user = await authentication.GetUserTypeAsync(guid);
			if (user == null || (user.UserType != UserType.Administrator))
			{
				return Unauthorized();
			}
			Show show = await showRepository.GetShowAsync(showUpdateDto.Id);
			if(show == null)
			{
				return NotFound();
			}
			//showRepository.DeleteShow(show);
			//await showRepository.SaveAsync();

			mapper.Map(showUpdateDto, show);
			showRepository.UpdateShow(show);
			await showRepository.SaveAsync();
			return NoContent();
		}
		/// <summary>
		/// 获取指定日期和影厅的有空闲的场次
		/// </summary>
		/// <param name="hallId"></param>
		/// <param name="date"></param>
		/// <returns></returns>
		[HttpGet("available/", Name = nameof(GetAvailableShows))]
		public async Task<ActionResult> GetAvailableShows(
			[FromQuery] int hallId,
			[FromQuery] DateTime date,
			[FromQuery] int? showId)
		{
			if (await hallRepository.GetHallAsync(hallId) == null)
			{
				return NotFound();
			}
			List<string> result = await showRepository.GetAvailableShowsAsync(date, hallId, showId);
			return Ok(result);
		}
		/// <summary>
		/// 根据Id获取场次
		/// </summary>
		/// <param name="showId"></param>
		/// <returns></returns>
		[HttpGet("{showId}", Name = nameof(GetShow))]
		public async Task<ActionResult<ShowDto>> GetShow(int showId)
		{
			Show show = await showRepository.GetShowAsync(showId);
			if (show == null)
			{
				return NotFound();
			}
			show.Movie = await movieRepository.GetMovieAsync(show.MovieId);
			show.Hall = await hallRepository.GetHallAsync(show.HallId);
			ShowDto showDto = mapper.Map<ShowDto>(show);
			return Ok(showDto);

		}
		/// <summary>
		/// 获取所有场次
		/// </summary>
		/// <returns></returns>
		[HttpGet(Name = nameof(GetShows))]
		public async Task<ActionResult> GetShows(
			[FromQuery] PagedParametersBase pagedParameters,
			[FromHeader(Name = "Accept")] string mediaType)
		{
			if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
			{
				return BadRequest();
			}
			//分页
			PagedListBase<Show> pagedShows = await showRepository.GetShowsAsync(pagedParameters);
			foreach (Show show in pagedShows)
			{
				show.Movie = await movieRepository.GetMovieAsync(show.MovieId);
				show.Hall = await hallRepository.GetHallAsync(show.HallId);
			}
			//转换
			List<ShowDto> pagedShowDtos = mapper.Map<List<ShowDto>>(pagedShows);
			//添加链接
			if (parsedMediaType.MediaType == "application/vnd.cinemaTicketing.hateoas+json")
			{
				List<dynamic> linkedShowDtos = new List<dynamic>();
				foreach (ShowDto data in pagedShowDtos)
				{
					IEnumerable<LinkDto> linksForShow = CreateLinksForHall(data.Id);
					linkedShowDtos.Add(new { data, linksForShow });
				}
				//添加翻页信息
				IEnumerable<LinkDto> linksForShows = CreateLinksForHalls(pagedParameters, pagedShows.HasPrevious, pagedShows.HasNext);
				var result = new { linkedShowDtos, linksForShows, pagedShows.TotalCount, pagedShows.CurrentPage, pagedShows.TotalPages };
				return Ok(result);
			}
			return Ok(pagedShowDtos);
		}
		/// <summary>
		/// 添加场次
		/// </summary>
		/// <param name="showAddDto"></param>
		/// <returns></returns>
		[HttpPost(Name = nameof(AddShow))]
		public async Task<ActionResult<Show>> AddShow(
			[FromBody] ShowAddDto showAddDto,
			[FromHeader(Name = "guid")] Guid guid)
		{
			//检查用户是否已经登录
			if (guid == Guid.Empty)
			{
				return Unauthorized();
			}
			//检查登陆的用户是否为管理员
			User user = await authentication.GetUserTypeAsync(guid);
			if (user == null || (user.UserType != UserType.Administrator))
			{
				return Unauthorized();
			}
			Show show = mapper.Map<Show>(showAddDto);
			//检查在当天该影厅是否存在同1场次
			IEnumerable<Show> showsOfHallAtOneDay = await showRepository.GetShowsOfHallAtOneDay(show.HallId, show.DateTime, show.ShowNum);
			if (showsOfHallAtOneDay.Count() != 0)
			{
				return UnprocessableEntity();
			}
			//添加场次，并返回结果
			Movie movie = await movieRepository.GetMovieAsync(showAddDto.MovieId);
			Hall hall = await hallRepository.GetHallAsync(showAddDto.HallId);
			show.Movie = movie;
			show.Hall = hall;
			showRepository.AddShow(show);
			await showRepository.SaveAsync();
			//Console.WriteLine(show.Expired);
			ShowDto showDto = mapper.Map<ShowDto>(show);
			return CreatedAtRoute(nameof(GetShows), new { showId = show.Id }, showDto);
		}
		/// <summary>
		/// 删除场次
		/// </summary>
		/// <param name="showId"></param>
		/// <param name="guid"></param>
		/// <returns></returns>
		[HttpDelete("{showId}", Name = nameof(DeleteShow))]
		public async Task<ActionResult> DeleteShow(
			int showId,
			[FromHeader(Name = "guid")] Guid guid)
		{
			//检查用户是否已经登录
			if (guid == Guid.Empty)
			{
				return Unauthorized();
			}
			//检查登陆的用户是否为管理员
			User user = await authentication.GetUserTypeAsync(guid);
			if (user == null || (user.UserType != UserType.Administrator))
			{
				return Unauthorized();
			}
			Show show = await showRepository.GetShowAsync(showId);
			if (show == null)
			{
				return NotFound();
			}
			showRepository.DeleteShow(show);
			await showRepository.SaveAsync();
			return NoContent();
		}
		/// <summary>
		/// 获取当天的所有场次
		/// </summary>
		/// <returns></returns>
		[HttpGet("today")]
		public async Task<ActionResult<IEnumerable<Show>>> GetShowsOfToday()
		{
			IEnumerable<Show> showsOfToday = await showRepository.GetShowsOfToday();
			IEnumerable<ShowDto> result = mapper.Map<IEnumerable<ShowDto>>(showsOfToday);
			return Ok(result);
		}
		/// <summary>
		/// 为show添加链接
		/// </summary>
		/// <param name="showId"></param>
		/// <returns></returns>
		private IEnumerable<LinkDto> CreateLinksForHall(int showId)
		{
			List<LinkDto> links = new List<LinkDto>
			{
				//new LinkDto(
				//	Url.Link(nameof(GetShow), new { showId }),
				//	"self",
				//	"Get"),
				new LinkDto(
					Url.Link(nameof(DeleteShow), new { showId }),
					"delete movie",
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
		private IEnumerable<LinkDto> CreateLinksForHalls(PagedParametersBase parameters, bool hasPrevious, bool hasNext)
		{
			List<LinkDto> links = new List<LinkDto>
			{
				new LinkDto(CreateHallsResourceUri(parameters, ResourceUriType.CurrentPage),
				"self",
				"Get")
			};
			if (hasPrevious)
			{
				links.Add(new LinkDto(CreateHallsResourceUri(parameters, ResourceUriType.PreviousPage),
					"previous_page",
					"Get"));
			}
			if (hasNext)
			{
				links.Add(new LinkDto(CreateHallsResourceUri(parameters, ResourceUriType.NextPage),
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
		private string CreateHallsResourceUri(PagedParametersBase parameters, ResourceUriType resourceUriType)
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
			return Url.Link(nameof(GetShows), new
			{
				pageNumber,
				pageSize = parameters.PageSize
			});
		}
	}
}
