using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Services
{
	
	public interface IUserRepository
	{
		/// <summary>
		/// 将修改应用到数据库
		/// </summary>
		/// <returns>是否执行成功</returns>
		Task<bool> SaveAsync();
		public Task<User> GetUserAsync(int userId);

		public Task<User> UserExists(User user);
	}
}
