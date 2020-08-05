using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaTicketing.Migrations
{
    public partial class removeAlternateKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Tickets_ShowId_SeatNum",
                table: "Tickets");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Shows_ShowNum_DateTime",
                table: "Shows");

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Halls",
                columns: new[] { "Id", "Name", "Seats" },
                values: new object[] { 2, "二号厅", 60 });

            migrationBuilder.InsertData(
                table: "Shows",
                columns: new[] { "Id", "DateTime", "HallId", "MovieId", "Price", "ShowNum" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 123.40000000000001, 1 },
                    { 2, new DateTime(2020, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 123.40000000000001, 2 },
                    { 5, new DateTime(2020, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 123.40000000000001, 3 },
                    { 6, new DateTime(2020, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 123.40000000000001, 1 },
                    { 7, new DateTime(2020, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 123.40000000000001, 2 },
                    { 8, new DateTime(2020, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 123.40000000000001, 1 },
                    { 9, new DateTime(2020, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 123.40000000000001, 2 },
                    { 10, new DateTime(2020, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 123.40000000000001, 3 },
                    { 11, new DateTime(2020, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 123.40000000000001, 4 },
                    { 12, new DateTime(2020, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 123.40000000000001, 5 },
                    { 13, new DateTime(2020, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 123.40000000000001, 6 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "UserName", "UserType" },
                values: new object[] { 3, "55587a910882016321201e6ebbc9f595", "李四", 1 });

            migrationBuilder.InsertData(
                table: "Shows",
                columns: new[] { "Id", "DateTime", "HallId", "MovieId", "Price", "ShowNum" },
                values: new object[] { 3, new DateTime(2020, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, 123.40000000000001, 3 });

            migrationBuilder.InsertData(
                table: "Shows",
                columns: new[] { "Id", "DateTime", "HallId", "MovieId", "Price", "ShowNum" },
                values: new object[] { 4, new DateTime(2020, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, 123.40000000000001, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ShowId",
                table: "Tickets",
                column: "ShowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_ShowId",
                table: "Tickets");

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Halls",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Tickets_ShowId_SeatNum",
                table: "Tickets",
                columns: new[] { "ShowId", "SeatNum" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Shows_ShowNum_DateTime",
                table: "Shows",
                columns: new[] { "ShowNum", "DateTime" });

            migrationBuilder.InsertData(
                table: "Shows",
                columns: new[] { "Id", "DateTime", "HallId", "MovieId", "Price", "ShowNum" },
                values: new object[] { 1, new DateTime(2020, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 123.40000000000001, 1 });
        }
    }
}
