﻿using CinemaTicketing.Models.Entity;
using CinemaTicketing.Services;
using System;
using System.Threading.Tasks;

namespace CinemaTicketing.Services.Impl
{
	public class Authentication : IAuthentication
	{
		private readonly IUserRepository userRepository;
		private readonly ILoggedUserRepository loggedUserRepository;

		public Authentication(ILoggedUserRepository loggedUserRepository, IUserRepository userRepository)
		{
			this.loggedUserRepository = loggedUserRepository ?? throw new ArgumentNullException(nameof(loggedUserRepository));
			this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		}
		public async Task<User> GetUserTypeAsync(Guid guid)
		{

			LoggedUser loggedUser = await loggedUserRepository.GetLoggedUserAsync(guid);
			if(loggedUser == null)
			{
				return null;
			}
			User user = await userRepository.GetUserAsync(loggedUser.UserId);
			return user;
		}
	}
}
