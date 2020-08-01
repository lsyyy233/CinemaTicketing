using CinemaTicketing.Models;
using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services
{
	public interface ILoggedUserRepository
	{
		public Task<LoggedUser> GetLoggedUserAsync(Guid guid);
		public void AddLoggedUser(LoggedUser loggedUser);
		public Task<LoggedUser> HasLoggedAsync(int userId);
		public Task<bool> SaveAsync();
		public void UpdateLoggedUser(LoggedUser loggedUser);
		public void DeleteLoggedUser(LoggedUser loggedUser);
	}
}
