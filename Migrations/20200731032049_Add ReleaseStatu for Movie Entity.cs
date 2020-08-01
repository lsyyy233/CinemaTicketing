using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaTicketing.Migrations
{
    public partial class AddReleaseStatuforMovieEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReleased",
                table: "Movies");

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderTheHit",
                table: "Movies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Introduction", "IsUnderTheHit", "Name" },
                values: new object[] { 5, "因故来到美国唐人街的叶问，意外卷入一场当地军方势力与华人武馆的纠纷，面对日益猖狂的民族歧视与压迫，叶问挺身而出，在美国海军陆战队军营拼死一战，以正宗咏春，向世界证明了中国功夫。", true, "叶问4 宗师传奇" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "IsUnderTheHit",
                table: "Movies");

            migrationBuilder.AddColumn<bool>(
                name: "IsReleased",
                table: "Movies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsReleased",
                value: true);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsReleased",
                value: true);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsReleased",
                value: true);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsReleased",
                value: true);
        }
    }
}
