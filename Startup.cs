using AutoMapper;
using CinemaTicketing.Helpers;
using CinemaTicketing.Models;
using CinemaTicketing.Services;
using CinemaTicketing.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace CinemaTicketing
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddRazorPages();
			services.AddControllers(setup =>
			{
				setup.ReturnHttpNotAcceptable = true;
			})
			.AddNewtonsoftJson(setup =>
			{
				setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			})
			.AddXmlDataContractSerializerFormatters();
			//识别application/vnd.company.hateoas+json Accept请求头
			services.Configure<MvcOptions>(config =>
			{
				NewtonsoftJsonOutputFormatter newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
				if (newtonsoftJsonOutputFormatter != null)
				{
					newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.cinemaTicketing.hateoas+json");
				}
			});
			services
				.AddScoped<IHallRepository, HallRepository>()
				.AddScoped<IMovieRepository, MovieRepository>()
				.AddScoped<ITicketRepository, TicketRepository>()
				.AddScoped<IUserRepository, UserRepository>()
				.AddScoped<IShowRepository, ShowRepository>()
				.AddScoped<ILoggedUserRepository,LoggedUserRepository>()
				.AddScoped<IAuthentication,Authentication>();
			services.AddDbContext<CinemaTicketingDbContext>();
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler(appBuilder =>
				{
					appBuilder.Run(async context =>
					{
						context.Response.StatusCode = 500;
						await context.Response.WriteAsync("Unexpected Error");
					});
				});
			}

			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
