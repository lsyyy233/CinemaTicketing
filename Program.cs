using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicketing.Helpers;
using CinemaTicketing.Models;
using CinemaTicketing.Services.Impl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CinemaTicketing
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IHost host = CreateHostBuilder(args).Build();
			using (var scope = host.Services.CreateScope())
			{
				try
				{
					CinemaTicketingDbContext dbContext = scope.ServiceProvider.GetService<CinemaTicketingDbContext>();
					dbContext.Database.EnsureDeleted();
					dbContext.Database.EnsureCreated();
				}
				catch (Exception e)
				{
					var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
					logger.LogError(e, "Database Migration Error");
				}
			}
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
