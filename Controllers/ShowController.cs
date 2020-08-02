using AutoMapper;
using CinemaTicketing.Helpers.Pagination;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
		private readonly IMovieRepository movieRepository;
		private readonly IHallRepository hallRepository;
		private readonly IMapper mapper;
		public ShowController(
			IMapper mapper,
			IShowRepository repository,
			IMovieRepository movieRepository,
			IHallRepository hallRepository)
		{
			this.mapper = mapper;
			this.showRepository = repository;
			this.movieRepository = movieRepository;
			this.hallRepository = hallRepository;
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
		public async Task<ActionResult<Show>> AddShow([FromBody]ShowAddDto showAddDto)
		{
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
		[HttpDelete("{showId}", Name = nameof(DeleteShow))]
		public async Task<ActionResult> DeleteShow(int showId)
		{
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
		/// 为Hall添加链接
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
		/// 为hall集合添加分页信息
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
