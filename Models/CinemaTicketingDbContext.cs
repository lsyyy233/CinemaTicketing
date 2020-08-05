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

			#endregion
			#region 设置主键
			modelBuilder.Entity<Hall>()
				.HasKey(x => x.Id);
			modelBuilder.Entity<Movie>()
				.HasKey(x => x.Id);
			modelBuilder.Entity<Show>()
				.HasKey(x => x.Id);
			modelBuilder.Entity<Ticket>()
				.HasKey(x => x.Id);
			modelBuilder.Entity<User>()
				.HasKey(x => x.Id);
			modelBuilder.Entity<LoggedUser>()
				.HasKey(x => x.UserId);
			#endregion
			#region 唯一约束
			////场次 日期、影厅和第*场的联合唯一约束
			//modelBuilder.Entity<Show>()
			//	.HasAlternateKey(x => new { x.ShowNum,x.HallId, x.DateTime });
			////电影票 场次和座位号唯一约束
			//modelBuilder.Entity<Ticket>()
			//	.HasAlternateKey(x => new { x.ShowId, x.SeatNum });
			#endregion
			#region 种子数据
			modelBuilder.Entity<Movie>()
				.HasData(
				new Movie
				{
					Id = 1,
					Name = "误杀",
					Introduction = "李维杰（肖央 饰）与妻子阿玉（谭卓 饰）打拼多年，膝下育有两个女儿。" +
					"一个雨夜，一场意外，打破了这个家庭的宁静。而李维杰作为一个父亲，为了保全自己的家人，他不顾一切地决定瞒天过海……",
					IsUnderTheHit = false
				},
				new Movie
				{
					Id = 2,
					Name = "寻梦环游记",
					Introduction = "热爱音乐的米格尔（安东尼·冈萨雷兹 Anthony Gonzalez 配音）不幸地出生在一个视音乐为洪水猛兽的大家庭之中，" +
					"一家人只盼着米格尔快快长大，好继承家里传承了数代的制鞋产业。" +
					"一年一度的亡灵节即将来临，每逢这一天，去世的亲人们的魂魄便可凭借着摆在祭坛上的照片返回现世和生者团圆。 " +
					"在一场意外中，米格尔竟然穿越到了亡灵国度之中，在太阳升起之前，他必须得到一位亲人的祝福，否则就将会永远地留在这个世界里。" +
					"米格尔决定去寻找已故的歌神德拉库斯（本杰明·布拉特 Benjamin Bratt 配音），因为他很有可能就是自己的祖父。" +
					"途中，米格尔邂逅了落魄乐手埃克托（盖尔·加西亚·贝纳尔 Gael García Bernal 配音），也渐渐发现了德拉库斯隐藏已久的秘密。",
					IsUnderTheHit = false
				},
				new Movie
				{
					Id = 3,
					Name = "多力特的奇幻冒险",
					Introduction = "维多利亚女王时代著名的医生兼兽医，异于常人的约翰·多力特医生在痛失爱妻七年之后，" +
					"隐居在多力特庄园的高墙内，只与自己饲养的珍稀动物为伴。" +
					"谁知年轻的女王（杰西·巴克利 饰）罹患重病，多力特只得启航，踏上一段恢宏旅程，前往一座神秘的岛屿寻找治病良方。" +
					"沿途历经重遇宿敌，发现神奇生物，他也重拾起了智慧和勇气。 与多力特一起踏上旅程的，" +
					"还有一位年轻的、自封的学徒（哈里·科莱特 饰）和一群七嘴八舌的动物朋友，其中有一只焦虑的大猩猩（拉米·马雷克 配音）、" +
					"一只热情但呆头呆脑的鸭子（奥克塔维亚·斯宾瑟 配音）、终日斗嘴的一只愤世嫉俗的鸵鸟（库梅尔·南贾尼 配音）和一头欢乐的北极熊（约翰·塞纳 配音），" +
					"以及一只顽固的鹦鹉（艾玛·汤普森 配音），它同时也是多力特最信任的顾问和知己。",
					IsUnderTheHit = false
				},
				new Movie
				{
					Id = 4,
					Name = "喋血战士",
					Introduction = "电影根据勇士漫画改编，讲述由范·迪塞尔扮演的士兵雷·加里森在战斗中意外阵亡，" +
					"被RST公司利用尖端纳米技术起死回生，变身即时自愈、媲美超级电脑的超级英雄——喋血战士。" +
					"苏醒的他执着于脑海中残存的记忆碎片，踏上为爱妻的复仇之路，却发现自己竟无法分清真实与虚幻...",
					IsUnderTheHit = false
				},
				new Movie
				{
					Id = 5,
					Name = "叶问4 宗师传奇",
					Introduction = "因故来到美国唐人街的叶问，意外卷入一场当地军方势力与华人武馆的纠纷，" +
					"面对日益猖狂的民族歧视与压迫，叶问挺身而出，在美国海军陆战队军营拼死一战，以正宗咏春，向世界证明了中国功夫。",
					IsUnderTheHit = true
				});
			modelBuilder.Entity<Hall>()
				.HasData(
				new Hall
				{
					Id = 1,
					Seats = 56,
					Name = "一号厅"
				},
				new Hall 
				{
					Id = 2,
					Seats = 60,
					Name = "二号厅"
				});
			modelBuilder.Entity<Show>()
				.HasData(
				new Show
				{
					Id = 1,
					DateTime = DateTime.Parse("2020-08-10T00:00:00"),
					ShowNum = ShowNum.第一场,
					MovieId = 1,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 2,
					DateTime = DateTime.Parse("2020-08-10T00:00:00"),
					ShowNum = ShowNum.第二场,
					MovieId = 2,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 3,
					DateTime = DateTime.Parse("2020-08-10T00:00:00"),
					ShowNum = ShowNum.第三场,
					MovieId = 3,
					HallId = 2,
					Price = 123.4
				},
				new Show
				{
					Id = 4,
					DateTime = DateTime.Parse("2020-08-11T00:00:00"),
					ShowNum = ShowNum.第一场,
					MovieId = 3,
					HallId = 2,
					Price = 123.4
				},
				new Show
				{
					Id = 5,
					DateTime = DateTime.Parse("2020-08-12T00:00:00"),
					ShowNum = ShowNum.第三场,
					MovieId = 3,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 6,
					DateTime = DateTime.Parse("2020-08-12T00:00:00"),
					ShowNum = ShowNum.第一场,
					MovieId = 2,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 7,
					DateTime = DateTime.Parse("2020-08-12T00:00:00"),
					ShowNum = ShowNum.第二场,
					MovieId = 5,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 8,
					DateTime = DateTime.Parse("2020-08-13T00:00:00"),
					ShowNum = ShowNum.第一场,
					MovieId = 5,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 9,
					DateTime = DateTime.Parse("2020-08-13T00:00:00"),
					ShowNum = ShowNum.第二场,
					MovieId = 5,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 10,
					DateTime = DateTime.Parse("2020-08-13T00:00:00"),
					ShowNum = ShowNum.第三场,
					MovieId = 5,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 11,
					DateTime = DateTime.Parse("2020-08-13T00:00:00"),
					ShowNum = ShowNum.第四场,
					MovieId = 5,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 12,
					DateTime = DateTime.Parse("2020-08-13T00:00:00"),
					ShowNum = ShowNum.第五场,
					MovieId = 5,
					HallId = 1,
					Price = 123.4
				},
				new Show
				{
					Id = 13,
					DateTime = DateTime.Parse("2020-08-13T00:00:00"),
					ShowNum = ShowNum.第六场,
					MovieId = 5,
					HallId = 1,
					Price = 123.4
				});
			modelBuilder.Entity<User>()
				.HasData(
				new User
				{
					Id = 1,
					UserName = "admin",
					Password = "123456",
					UserType = UserType.Administrator
				});
			modelBuilder.Entity<User>()
				.HasData(
				new User
				{
					Id = 2,
					UserName = "张三",
					Password = "123789",
					UserType = UserType.RegularUser
				},
				new User
				{
					Id = 3,
					UserName = "李四",
					Password = "123789",
					UserType = UserType.RegularUser
				});
			#endregion
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder
#if DEBUG
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
			builder.AddFilter((category, level) =>
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
