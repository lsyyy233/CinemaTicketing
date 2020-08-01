using AutoMapper;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using CinemaTicketing.Services.Impl;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinemaTicketing.Controllers
{
	[ApiController]
	[Route("api/halls")]
	public class HallController : ControllerBase
	{
		private readonly IHallRepository hallRepository;
		private readonly IMapper mapper;
		public HallController(
			IMapper mapper, 
			IHallRepository hallRepository)
		{
			this.mapper = mapper;
			this.hallRepository = hallRepository;
		}
		/// <summary>
		/// 根据Id获取场次
		/// </summary>
		/// <param name="hallId"></param>
		/// <returns></returns>
		[HttpGet("{hallId}", Name = nameof(GetHall))]
		public async Task<ActionResult> GetHall([FromRoute] int hallId)
		{
			Hall hall = await hallRepository.GetHall(hallId);
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
		[HttpPost(Name =nameof(AddHall))]
		public async Task<ActionResult> AddHall([FromBody]HallAddDto hallAddDto)
		{
			Hall hall = mapper.Map<Hall>(hallAddDto);
			hallRepository.AddHall(hall);
			await hallRepository.SaveAsync();
			HallDto hallDto = mapper.Map<HallDto>(hall);
			return CreatedAtRoute(
				nameof(AddHall), 
				new { hallId = hall.Id }, 
				hallDto );
		}
	}
}
