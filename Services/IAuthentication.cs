using CinemaTicketing.Models.Entity;
using System;
using System.Threading.Tasks;

namespace CinemaTicketing.Services
{
	public interface IAuthentication
	{
		Task<User> GetUserTypeAsync(Guid guid);
	}
}