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
using System.Threading.Tasks;

namespace CinemaTicketing.Controllers
{
	[ApiController]
	[Route("api/halls")]
	public class HallController : ControllerBase
	{
		private readonly IHallRepository hallRepository;
		private readonly IMapper mapper;
		private readonly IAuthentication authentication;
		public HallController(
			IMapper mapper,
			IHallRepository hallRepository, 
			IAuthentication authentication)
		{
			this.mapper = mapper;
			this.hallRepository = hallRepository;
			this.authentication = authentication;
		}
		
		/// <summary>
		/// 获取在指定日期没有排满场次的影厅
		/// </summary>
		/// <param name = "date" ></ param >
		/// < returns ></ returns >
		[HttpGet("date/{date}", Name = nameof(GetHallOfDateAsync))]
		public async Task<ActionResult> GetHallOfDateAsync(DateTime date)
		{
			List<Hall> halls = await hallRepository.GetHallOfDateAsync(date);
			List<HallDto> hallDtos = mapper.Map<List<HallDto>>(halls);
			return Ok(hallDtos);
		}
		/// <summary>
		/// 更新影厅
		/// </summary>
		/// <param name="hallUpdateDto"></param>
		/// <param name="guid"></param>
		/// <returns></returns>
		[HttpPut(Name = nameof(UpdateHallAsync))]
		public async Task<ActionResult> UpdateHallAsync([
			FromBody]HallUpdateDto hallUpdateDto,
			[FromHeader] Guid guid)
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
			Hall hall = await hallRepository.GetHallAsync(hallUpdateDto.Id);
			if (hall == null)
			{
				return NotFound();
			}
			mapper.Map(hallUpdateDto, hall);
			hallRepository.UpdateHall(hall);
			await hallRepository.SaveAsync();
			return NoContent();
		}
		/// <summary>
		/// 根据Id获取场次
		/// </summary>
		/// <param name="hallId"></param>
		/// <returns></returns>
		[HttpGet("{hallId}", Name = nameof(GetHall))]
		public async Task<ActionResult> GetHall([FromRoute] int hallId)
		{
			Hall hall = await hallRepository.GetHallAsync(hallId);
			if (hall == null)
			{
				return NotFound();
			}
			HallDto hallDto = mapper.Map<HallDto>(hall);
			return Ok(hallDto);
		}
		/// <summary>
		/// 添加场次
		/// </summary>
		/// <param name="hallAddDto"></param>
		/// <returns></returns>
		[HttpPost(Name = nameof(AddHall))]
		public async Task<ActionResult> AddHall(
			[FromBody] HallAddDto hallAddDto,
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
			Hall hall = mapper.Map<Hall>(hallAddDto);
			hallRepository.AddHall(hall);
			await hallRepository.SaveAsync();
			HallDto hallDto = mapper.Map<HallDto>(hall);
			return CreatedAtRoute(
				nameof(AddHall),
				new { hallId = hall.Id },
				hallDto);
		}
		/// <summary>
		/// 查询所有Hall
		/// </summary>
		/// <returns></returns>
		[HttpGet(Name = nameof(GetHalls))]
		public async Task<ActionResult<List<HallDto>>> GetHalls(
			[FromQuery] PagedParametersBase pagedParameters,
			[FromHeader(Name = "Accept")] string mediaType)
		{
			if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
			{
				return BadRequest();
			}
			//分页
			PagedListBase<Hall> pagedHalls = await hallRepository.GetHallsAsync(pagedParameters);
			//转换
			List<HallDto> pagedHallDtos = mapper.Map<List<HallDto>>(pagedHalls);
			//添加链接
			if (parsedMediaType.MediaType == "application/vnd.cinemaTicketing.hateoas+json")
			{
				List<dynamic> linkedHallDtos = new List<dynamic>();
				foreach (HallDto data in pagedHallDtos)
				{
					IEnumerable<LinkDto> linksForHall = CreateLinksForHall(data.Id);
					linkedHallDtos.Add(new { data, linksForHall });
				}
				//添加翻页信息
				IEnumerable<LinkDto> linksForHalls = CreateLinksForHalls(pagedParameters, pagedHalls.HasPrevious, pagedHalls.HasNext);
				var result = new { linkedHallDtos, linksForHalls, pagedHalls.TotalCount, pagedHalls.CurrentPage, pagedHalls.PageSize, pagedHalls.TotalPages };
				return Ok(result);
			}
			return Ok(pagedHallDtos);
		}
		/// <summary>
		/// 删除hall
		/// </summary>
		/// <param name="hallId"></param>
		/// <returns></returns>
		[HttpDelete("{hallId}", Name = nameof(DeleteHall))]
		public async Task<ActionResult> DeleteHall(
			[FromRoute] int hallId,
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
			Hall hall = await hallRepository.GetHallAsync(hallId);
			if (hall == null)
			{
				return NotFound();
			}
			hallRepository.DeleteHall(hall);
			await hallRepository.SaveAsync();
			return NoContent();
		}
		/// <summary>
		/// 为单个Hall添加链接
		/// </summary>
		/// <param name="movieId"></param>
		/// <returns></returns>
		private IEnumerable<LinkDto> CreateLinksForHall(int hallId)
		{
			List<LinkDto> links = new List<LinkDto>
			{
				//new LinkDto(
				//	Url.Link(nameof(GetHall), new { hallId }),
				//	"self",
				//	"Get"),
				new LinkDto(
					Url.Link(nameof(DeleteHall), new { hallId }),
					"delete movie",
					"Delete")
			};
			return links;
		}
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
			return Url.Link(nameof(GetHalls), new
			{
				pageNumber,
				pageSize = parameters.PageSize
			});
		}
	}
}
