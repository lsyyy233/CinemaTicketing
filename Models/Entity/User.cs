using Masuit.Tools.Security;
using System;
using System.Collections.Generic;

namespace CinemaTicketing.Models.Entity
{
	/// <summary>
	/// 用户
	/// </summary>
	public enum UserType
	{
		Administrator = 0,
		RegularUser = 1
	}
	public class User
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		private string password;
		private UserType userType = UserType.RegularUser;
		public LoggedUser Logged { get; set; }
		//public Guid LoggedGuid { get; set; }
		public UserType UserType { get => userType; set => userType = value; }
		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value.MDString();
			}
		}
		public List<Ticket> Tickets { get; set; }
	}
}
