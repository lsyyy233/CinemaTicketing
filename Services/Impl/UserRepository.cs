﻿using CinemaTicketing.Models;
using CinemaTicketing.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services.Impl
{
	public class UserRepository :  IUserRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;
		public UserRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext;
		}

		public async Task<User> GetUserAsync(int userId)
		{
			return await _DbContext.Users
				.Where(x => x.Id == userId)
				.SingleOrDefaultAsync();
		}

		public async Task<bool> SaveAsync()
		{
			return await _DbContext.SaveChangesAsync() >= 0;
		}

		public async Task<User> UserExists(User user)
		{
			return await _DbContext.Users
				.Where(x => x.UserName == user.UserName && x.Password == user.Password)
				.SingleOrDefaultAsync();
		}
	}
}