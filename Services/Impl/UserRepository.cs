using CinemaTicketing.Models;
using CinemaTicketing.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services.Impl
{
	public class UserRepository : IUserRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;
		public UserRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext ?? throw new ArgumentNullException(nameof(cinemaTicketingDbContext));
		}

		public void AddUser(User user)
		{
			//try
			//{
			//	user.Id = _DbContext.Users.Select(x => x.Id).Max() + 1;
			//}
			//catch (System.InvalidOperationException)
			//{
			//	user.Id = 1;
			//}

			_DbContext.Users.Add(user);
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

		public async Task<bool> UserNameExistsAsync(string userName)
		{
			User user = await _DbContext.Users
				.Where(x => x.UserName == userName)
				.SingleOrDefaultAsync();
			return user != null;
		}

		public async Task<User> UserExistsAsync(User user)
		{
			return await _DbContext.Users
				.Where(x => x.UserName == user.UserName && x.Password == user.Password)
				.SingleOrDefaultAsync();
		}
	}
}
