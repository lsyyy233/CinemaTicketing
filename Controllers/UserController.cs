using AutoMapper;
using CinemaTicketing.Helpers;
using CinemaTicketing.Models.Dtos;
using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CinemaTicketing.Controllers
{
	[ApiController]
	[Route("api/users")]
	public class UserController : ControllerBase
	{
		private readonly ILoggedUserRepository loggedUserRepository;
		private readonly IUserRepository userRepository;
		private readonly IMapper mapper;
		public UserController(
			ILoggedUserRepository loggedUserRepository,
			IUserRepository userRepository,
			IMapper mapper)
		{
			this.loggedUserRepository = loggedUserRepository;
			this.userRepository = userRepository;
			this.mapper = mapper;
		}
		[HttpGet(Name =nameof(GetUserById))]
		public async Task<ActionResult> GetUserById(int userId)
		{
			User user =  await userRepository.GetUserAsync(userId);
			UserDto userDto = mapper.Map<UserDto>(user);
			return Ok(userDto);
		}
		/// <summary>
		/// 用户注册
		/// </summary>
		/// <param name="userAddDto"></param>
		/// <returns></returns>
		[HttpPost(Name =nameof(UserRegister))]
		public async Task<ActionResult> UserRegister([FromBody]UserAddDto userAddDto)
		{
			if (await userRepository.UserNameExistsAsync(userAddDto.UserName))
			{
				return Conflict();
			}
			User user = mapper.Map<User>(userAddDto);
			userRepository.AddUser(user);
			await userRepository.SaveAsync();
			UserDto userDto = mapper.Map<UserDto>(user);
			return CreatedAtRoute(
				nameof(GetUserById),
				user.Id,
				userDto);
		}
		/// <summary>
		/// 用户登录
		/// </summary>
		/// <param name="userLoginDto"></param>
		/// <returns>如果登陆成功，返回LoggedUserDto对象</returns>
		[HttpPost("login/", Name = nameof(LoginIn))]
		public async Task<ActionResult<LoggedUserDto>> LoginIn([FromBody] UserLoginDto userLoginDto, [FromHeader(Name = "Guid")] string text)
		{
			Console.WriteLine(text);
			LoggedUserDto loggedUserDto;
			UserDto userDto;
			User user = mapper.Map<User>(userLoginDto);
			//检查用户是否存在
			User existsUser = await userRepository.UserExistsAsync(user);
			if (existsUser == null)
			{
				//不存在该用户，返回422
				return UnprocessableEntity();
			}
			LoggedUser loggedUser = await loggedUserRepository.HasLoggedAsync(existsUser.Id);
			//如果存在，检查LoggerUser中该用户是否已经登录
			if (loggedUser != null)
			{
				//如果已经登录，更新登录信息(Guid)
				loggedUser.Guid = Guid.NewGuid();
				await loggedUserRepository.SaveAsync();
				//构建返回信息
				loggedUserDto = mapper.Map<LoggedUserDto>(loggedUser);
				userDto = mapper.Map<UserDto>(existsUser);
				loggedUserDto.UserDto = userDto;
				return Ok(loggedUserDto);
			}
			//生成登录记录
			LoggedUser newLoggedUser = new LoggedUser
			{
				UserId = existsUser.Id,
				//Guid = Guid.NewGuid()
			};
			loggedUserRepository.AddLoggedUser(newLoggedUser);
			await loggedUserRepository.SaveAsync();
			//返回登录信息
			loggedUserDto = mapper.Map<LoggedUserDto>(newLoggedUser);
			userDto = mapper.Map<UserDto>(existsUser);
			loggedUserDto.UserDto = userDto;
			//return Ok(loggedUserDto);
			return Ok(loggedUserDto);
		}
		/// <summary>
		/// 根据Guid获取已登录的用户信息
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		[HttpGet("{guid}", Name = nameof(GetLoggedUserAsync))]
		public async Task<ActionResult<LoggedUserDto>> GetLoggedUserAsync(Guid guid)
		{
			LoggedUser loggedUser = await loggedUserRepository.GetLoggedUserAsync(guid);
			if (loggedUser == null)
			{
				return NotFound();
			}
			LoggedUserDto loggedUserDto = mapper.Map<LoggedUserDto>(loggedUser);
			User user = await userRepository.GetUserAsync(loggedUser.UserId);
			UserDto userDto = mapper.Map<UserDto>(user);
			loggedUserDto.UserDto = userDto;
			return Ok(loggedUserDto);
		}

	}
}
