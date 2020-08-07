using CinemaTicketing.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using System;

namespace CinemaTicketing.Models
{
	public class CinemaTicketingDbContext : DbContext
	{
		#region DbSet
		public DbSet<Hall> Halls { get; set; }
		public DbSet<Movie> Movies { get; set; }
		//public DbSet<SeatOfShow> SeatsOfShow { get; set; }
		public DbSet<Show> Shows { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<LoggedUser> LoggedUsers { get; set; }
		#endregion
		public CinemaTicketingDbContext(DbContextOptions<CinemaTicketingDbContext> options)
			: base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<Show>()
			//	.HasIndex(x => x.Id)
			//	.IsUnique();

			#region 对应关系
			//Ticket和Show的对应关系
			//一个TicketTemplates对应多个Ticket
			modelBuilder.Entity<Ticket>()
				.HasOne(x => x.Show)
				.WithMany(x => x.Tickets)
				.HasForeignKey(x => x.ShowId);
			//Ticket和user的对应关系
			//一个user对应多个Ticket
			modelBuilder.Entity<Ticket>()
				.HasOne(x => x.User)
				.WithMany(x => x.Tickets)
				.HasForeignKey(x => x.UserId);
			//Hall Show一对多
			modelBuilder.Entity<Show>()
				.HasOne(x => x.Hall)
				.WithMany(x => x.Shows)
				.HasForeignKey(x => x.HallId);
			//Movie show 一对多
			modelBuilder.Entity<Show>()
				.HasOne(x => x.Movie)
				.WithMany(x => x.Shows)
				.HasForeignKey(x => x.MovieId);
			//user LoggedUser 一对一
			modelBuilder.Entity<LoggedUser>()
				.HasOne(x => x.User)
				.WithOne(x => x.Logged)
				.HasForeignKey<LoggedUser>(x => x.UserId);
			//设置LoggedUser主键
			modelBuilder.Entity<LoggedUser>()
				.HasKey(x => x.UserId);

			#endregion
			//#region 设置主键
			//modelBuilder.Entity<Hall>()
			//	.HasKey(x => x.Id);
			//modelBuilder.Entity<Movie>()
			//	.HasKey(x => x.Id);
			//modelBuilder.Entity<Show>()
			//	.HasKey(x => x.Id);
			//modelBuilder.Entity<Ticket>()
			//	.HasKey(x => x.Id);
			//modelBuilder.Entity<User>()
			//	.HasKey(x => x.Id);
			//#endregion
			//#region 唯一约束
			////场次 日期、影厅和第*场的联合唯一约束
			//modelBuilder.Entity<Show>()
			//	.HasAlternateKey(x => new { x.ShowNum,x.HallId, x.DateTime });
			////电影票 场次和座位号唯一约束
			//modelBuilder.Entity<Ticket>()
			//	.HasAlternateKey(x => new { x.ShowId, x.SeatNum });
			//#endregion
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
#if DEBUG
			optionsBuilder
				//将生成的SQL语句打印到控制台
				.UseLoggerFactory(ConsoleLoggerFactory)
				.EnableSensitiveDataLogging();
#endif
			//.UseMySql(GetConnectString());
		}
		/// <summary>
		/// 读取appsetting.json中的链接字符串
		/// </summary>
		/// <returns>数据库连接字符串</returns>
		//public static string GetConnectString()
		//{
		//	ConfigurationBuilder builder = new ConfigurationBuilder();
		//	builder.SetBasePath(System.Environment.CurrentDirectory).AddJsonFile("appsettings.json", optional: false);
		//	IConfigurationRoot configuration = builder.Build();
		//	string connectionString = configuration.GetConnectionString("BloggingDatabase");
		//	return connectionString;
		//}
		public static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder =>
		{
			builder
			.AddFilter((category, level) =>
				category == DbLoggerCategory.Database.Command.Name
				&&
				level == LogLevel.Information)
			.AddConsole();
		});
		public static readonly LoggerFactory DebugLoggerFactory = new LoggerFactory(new[] {
			new DebugLoggerProvider()
		});

	}
}
