using CinemaTicketing.Models;
using CinemaTicketing.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services.Impl
{

	public class LoggedUserRepository : ILoggedUserRepository
	{
		protected readonly CinemaTicketingDbContext _DbContext;

		public LoggedUserRepository(CinemaTicketingDbContext cinemaTicketingDbContext)
		{
			_DbContext = cinemaTicketingDbContext;
		}
		/// <summary>
		/// 用户登录
		/// </summary>
		/// <param name="loggedUser"></param>
		public void AddLoggedUser(LoggedUser loggedUser)
		{
			if (loggedUser == null)
			{
				throw new ArgumentNullException(nameof(loggedUser));
			}
			loggedUser.Guid = Guid.NewGuid();
			_DbContext.LoggedUsers.Add(loggedUser);
		}
		/// <summary>
		/// 用户注销登录
		/// </summary>
		/// <param name="loggedUser"></param>
		public void DeleteLoggedUser(LoggedUser loggedUser)
		{
			if (loggedUser == null)
			{
				throw new ArgumentNullException(nameof(loggedUser));
			}
			loggedUser.Guid = new Guid();
			_DbContext.LoggedUsers.Remove(loggedUser);
		}
		public async Task<LoggedUser> GetLoggedUserAsync(Guid guid)
		{
			return await _DbContext.LoggedUsers
				.Where(x => x.Guid == guid)
				.SingleOrDefaultAsync();
		}
		/// <summary>
		/// 检查用户是否已经登录
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<LoggedUser> HasLoggedAsync(int userId)
		{
			return await _DbContext.LoggedUsers
				.Where(x => x.UserId == userId)
				.SingleOrDefaultAsync();
		}

		public async Task<bool> SaveAsync()
		{
			return await _DbContext.SaveChangesAsync() >= 0;
		}

		public void UpdateLoggedUser(LoggedUser loggedUser)
		{
		}
	}
}
