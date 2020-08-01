using AutoMapper;
using CinemaTicketing.Helpers;
using CinemaTicketing.Helpers.Pagination;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Dtos.UpdateDtos;
using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using CinemaTicketing.Services.Impl;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CinemaTicketing.Controllers
{
	[Route("api/movies/")]
	[ApiController]
	public class MovieController : ControllerBase
	{
		private readonly IMovieRepository movieRepository;
		private readonly IShowRepository showRepository;
		private readonly IAuthentication authentication;
		private readonly IMapper mapper;
		public MovieController(
			IMovieRepository repository,
			IMapper mapper, IShowRepository showRepository, IAuthentication authentication = null)
		{
			this.movieRepository = repository ?? throw new ArgumentNullException(nameof(repository));
			this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
			this.showRepository = showRepository;
			this.authentication = authentication;
		}
		/// <summary>
		/// 获取所有电影信息
		/// </summary>
		/// <returns></returns>
		[HttpGet(Name = nameof(GetMovies))]
		public async Task<ActionResult> GetMovies([FromQuery] PagedParametersBase pagedParameters, [FromHeader(Name = "Accept")] string mediaType)
		{
			//
			if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
			{
				return BadRequest();
			}
			//对数据进行分页
			PagedListBase<Movie> pagedMovies = await movieRepository.GetMoviesAsync(pagedParameters);
			//转换为MovieDto
			List<MovieDto> movieDtos = new List<MovieDto>();
			//设置MovieDto中的ReleaseStatu属性
			foreach (Movie movie in pagedMovies)
			{
				MovieDto movieDto = mapper.Map<MovieDto>(movie);
				if (movie.IsUnderTheHit)
				{
					movieDto.ReleaseStatu = ReleaseStatu.已经下映;
				}
				else if (showRepository.GetShowsNotExpire(movieDto.Id).Result.Count != 0)
				{
					movieDto.ReleaseStatu = ReleaseStatu.正在上映;
				}
				else
				{
					movieDto.ReleaseStatu = ReleaseStatu.暂无场次;
				}
				movieDtos.Add(movieDto);
			}
			//添加hateoas信息
			if (parsedMediaType.MediaType == "application/vnd.company.hateoas+json")
			{
				List<dynamic> linkedMovieDto = new List<dynamic>();
				//遍历集合，为每一个元素添加hateoas信息
				foreach (MovieDto data in movieDtos)
				{
					IEnumerable<LinkDto> links = CreateLinksForMovie(data.Id);
					linkedMovieDto.Add(new { data, links });
				}
				//添加翻页信息
				IEnumerable<LinkDto> linksForMovies = CreateLinksForMovies(pagedParameters, pagedMovies.HasPrevious, pagedMovies.HasNext);
				var result = new { linkedMovieDto, linksForMovies , pagedMovies.TotalCount, pagedMovies.CurrentPage, pagedMovies.TotalPages };
				return Ok(result);
			}
			return Ok(movieDtos);
		}
		/// <summary>
		/// 根据Id获取电影信息
		/// </summary>
		/// <param name="movieId"></param>
		/// <returns></returns>
		[HttpGet("{movieId}", Name = nameof(GetMovie))]
		public async Task<IActionResult> GetMovie(int movieId)
		{
			
			Movie movie = await movieRepository.GetMovieAsync(movieId);
			if (movie == null)
			{
				return NotFound();
			}
			MovieDto movieDto = mapper.Map<MovieDto>(movie);
			if (movie.IsUnderTheHit)
			{
				movieDto.ReleaseStatu = ReleaseStatu.已经下映;
			}
			return Ok(movieDto);
		}
		/// <summary>
		/// 删除电影
		/// </summary>
		/// <param name="movieId"></param>
		/// <returns></returns>
		[HttpDelete("{movieId}", Name = nameof(DeleteMovie))]
		public async Task<IActionResult> DeleteMovie(int movieId)
		{
			Movie movie = await movieRepository.GetMovieAsync(movieId);
			if (movie == null)
			{
				return NotFound();
			}
			movieRepository.DeleteMovie(movie);
			await movieRepository.SaveAsync();
			return NoContent();
		}
		/// <summary>
		/// 添加电影
		/// </summary>
		/// <param name="movieAddDto"></param>
		/// <returns></returns>
		[HttpPost(Name = nameof(AddMovie))]
		public async Task<ActionResult> AddMovie([FromBody] MovieAddDto movieAddDto, [FromHeader(Name = "guid")] Guid guid)
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
			Movie movie = mapper.Map<Movie>(movieAddDto);
			movieRepository.AddMovieAsync(movie);
			await movieRepository.SaveAsync();
			MovieDto result = mapper.Map<MovieDto>(movie);
			return CreatedAtRoute(nameof(GetMovie), new { movieId = movie.Id }, result);
		}
		private IEnumerable<LinkDto> CreateLinksForMovie(int movieId)
		{
			List<LinkDto> links = new List<LinkDto>
			{
				new LinkDto(
					Url.Link(nameof(GetMovie), new { movieId }),
					"self",
					"Get"),
				new LinkDto(
					Url.Link(nameof(DeleteMovie), new { movieId }),
					"delete movie",
					"Delete")
			};
			return links;
		}
		/// <summary>
		/// 为集合添加翻页信息
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="hasPrevious"></param>
		/// <param name="hasNext"></param>
		/// <returns></returns>
		private IEnumerable<LinkDto> CreateLinksForMovies(PagedParametersBase parameters, bool hasPrevious, bool hasNext)
		{
			List<LinkDto> links = new List<LinkDto>
			{
				new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.CurrentPage),
				"self",
				"Get")
			};
			if (hasPrevious)
			{
				links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage),
					"previous_page",
					"Get"));
			}
			if (hasNext)
			{
				links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage),
					"next_page",
					"Get"));
			}
			return links;
		}
		/// <summary>
		/// 生成翻页链接
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="resourceUriType"></param>
		/// <returns></returns>
		private string CreateCompaniesResourceUri(PagedParametersBase parameters, ResourceUriType resourceUriType)
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
			return Url.Link(nameof(GetMovies), new
			{
				pageNumber,
				pageSize = parameters.PageSize
			});
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="movieUpdateDto"></param>
		/// <returns></returns>
		[HttpPut]
		public async Task<ActionResult> UpdateMovie(
			[FromHeader(Name ="guid")]Guid guid,
			[FromBody]MovieUpdateDto movieUpdateDto)
		{
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
			Movie movie = await movieRepository.GetMovieAsync(movieUpdateDto.Id);
			mapper.Map(movieUpdateDto, movie);
			movieRepository.UpdateMovie(movie);
			await movieRepository.SaveAsync();
			return NoContent();
		}
	}
}
