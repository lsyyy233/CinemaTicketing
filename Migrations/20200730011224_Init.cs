using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaTicketing.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Halls",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Seats = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Introduction = table.Column<string>(nullable: true),
                    IsReleased = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    ShowNum = table.Column<int>(nullable: false),
                    MovieId = table.Column<int>(nullable: false),
                    HallId = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                    table.UniqueConstraint("AK_Shows_ShowNum_DateTime", x => new { x.ShowNum, x.DateTime });
                    table.ForeignKey(
                        name: "FK_Shows_Halls_HallId",
                        column: x => x.HallId,
                        principalTable: "Halls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shows_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SeatNum = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    ShowId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.UniqueConstraint("AK_Tickets_ShowId_SeatNum", x => new { x.ShowId, x.SeatNum });
                    table.ForeignKey(
                        name: "FK_Tickets_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Halls",
                columns: new[] { "Id", "Name", "Seats" },
                values: new object[] { 1, "一号厅", 56 });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Introduction", "IsReleased", "Name" },
                values: new object[,]
                {
                    { 1, "李维杰（肖央 饰）与妻子阿玉（谭卓 饰）打拼多年，膝下育有两个女儿。一个雨夜，一场意外，打破了这个家庭的宁静。而李维杰作为一个父亲，为了保全自己的家人，他不顾一切地决定瞒天过海……", true, "误杀" },
                    { 2, "热爱音乐的米格尔（安东尼·冈萨雷兹 Anthony Gonzalez 配音）不幸地出生在一个视音乐为洪水猛兽的大家庭之中，一家人只盼着米格尔快快长大，好继承家里传承了数代的制鞋产业。一年一度的亡灵节即将来临，每逢这一天，去世的亲人们的魂魄便可凭借着摆在祭坛上的照片返回现世和生者团圆。 在一场意外中，米格尔竟然穿越到了亡灵国度之中，在太阳升起之前，他必须得到一位亲人的祝福，否则就将会永远地留在这个世界里。米格尔决定去寻找已故的歌神德拉库斯（本杰明·布拉特 Benjamin Bratt 配音），因为他很有可能就是自己的祖父。途中，米格尔邂逅了落魄乐手埃克托（盖尔·加西亚·贝纳尔 Gael García Bernal 配音），也渐渐发现了德拉库斯隐藏已久的秘密。", true, "寻梦环游记" },
                    { 3, "维多利亚女王时代著名的医生兼兽医，异于常人的约翰·多力特医生在痛失爱妻七年之后，隐居在多力特庄园的高墙内，只与自己饲养的珍稀动物为伴。谁知年轻的女王（杰西·巴克利 饰）罹患重病，多力特只得启航，踏上一段恢宏旅程，前往一座神秘的岛屿寻找治病良方。沿途历经重遇宿敌，发现神奇生物，他也重拾起了智慧和勇气。 与多力特一起踏上旅程的，还有一位年轻的、自封的学徒（哈里·科莱特 饰）和一群七嘴八舌的动物朋友，其中有一只焦虑的大猩猩（拉米·马雷克 配音）、一只热情但呆头呆脑的鸭子（奥克塔维亚·斯宾瑟 配音）、终日斗嘴的一只愤世嫉俗的鸵鸟（库梅尔·南贾尼 配音）和一头欢乐的北极熊（约翰·塞纳 配音），以及一只顽固的鹦鹉（艾玛·汤普森 配音），它同时也是多力特最信任的顾问和知己。", true, "多力特的奇幻冒险" },
                    { 4, "电影根据勇士漫画改编，讲述由范·迪塞尔扮演的士兵雷·加里森在战斗中意外阵亡，被RST公司利用尖端纳米技术起死回生，变身即时自愈、媲美超级电脑的超级英雄——喋血战士。苏醒的他执着于脑海中残存的记忆碎片，踏上为爱妻的复仇之路，却发现自己竟无法分清真实与虚幻...", true, "喋血战士" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "UserName", "UserType" },
                values: new object[,]
                {
                    { 1, "e10adc3949ba59abbe56e057f20f883e", "卢本伟", 0 },
                    { 2, "55587a910882016321201e6ebbc9f595", "张三", 1 }
                });

            migrationBuilder.InsertData(
                table: "Shows",
                columns: new[] { "Id", "DateTime", "HallId", "MovieId", "Price", "ShowNum" },
                values: new object[] { 1, new DateTime(2020, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 123.40000000000001, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Shows_HallId",
                table: "Shows",
                column: "HallId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_MovieId",
                table: "Shows",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Halls");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
