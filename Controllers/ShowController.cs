using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Dtos.AddDtos;
using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using CinemaTicketing.Services.Impl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
		[HttpGet("{showId}",Name =nameof(GetShow))]
		public async Task<ActionResult<ShowDto>> GetShow(int showId)
		{
			Show show = await showRepository.GetShowAsync(showId);
			if(show == null)
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
		public async Task<ActionResult<Show>> GetShows()
		{
			IEnumerable<Show> shows = await showRepository.GetShowsAsync();
			IEnumerable<ShowDto> showDtos = mapper.Map<IEnumerable<ShowDto>>(shows);
			return Ok(showDtos);
		}
		/// <summary>
		/// 添加场次
		/// </summary>
		/// <param name="showAddDto"></param>
		/// <returns></returns>
		[HttpPost(Name =nameof(AddShow))]
		public async Task<ActionResult<Show>> AddShow([FromBody]ShowAddDto showAddDto)
		{
			Show show = mapper.Map<Show>(showAddDto);
			//检查在当天该影厅是否存在同1场次
			IEnumerable<Show> showsOfHallAtOneDay = await showRepository.GetShowsOfHallAtOneDay(show.HallId, show.DateTime, show.ShowNum);
			if(showsOfHallAtOneDay.Count() != 0)
			{
				return UnprocessableEntity();
			}
			//添加场次，并返回结果
			Movie movie = await movieRepository.GetMovieAsync(showAddDto.MovieId);
			Hall hall = await hallRepository.GetHall(showAddDto.HallId);
			show.Movie = movie;
			show.Hall = hall;
			showRepository.AddShow(show);
			await showRepository.SaveAsync();
			//Console.WriteLine(show.Expired);
			ShowDto showDto = mapper.Map<ShowDto>(show);
			return CreatedAtRoute(nameof(GetShows), new { showId = show.Id }, showDto);
		}
		/// <summary>
		/// 获取当天的所有场次
		/// </summary>
		/// <returns></returns>
		[HttpGet("today")]
		public async Task<ActionResult<IEnumerable<Show>>> GetShowsOfToday()
		{
			IEnumerable<Show> showsOfToday =  await showRepository.GetShowsOfToday();
			IEnumerable<ShowDto> result = mapper.Map<IEnumerable<ShowDto>>(showsOfToday);
			return Ok(result);
		}
	}
}
