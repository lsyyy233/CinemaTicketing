using CinemaTicketing.Models.Entity;
using System;
using System.Threading.Tasks;

namespace CinemaTicketing.Helpers
{
	public interface IAuthentication
	{
		Task<User> GetUserTypeAsync(Guid guid);
	}
}