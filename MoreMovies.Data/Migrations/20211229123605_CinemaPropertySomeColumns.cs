using Microsoft.EntityFrameworkCore.Migrations;

namespace MoreMovies.Data.Migrations
{
    public partial class CinemaPropertySomeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Cinemas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Cinemas_UserId",
                table: "Cinemas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cinemas_AspNetUsers_UserId",
                table: "Cinemas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cinemas_AspNetUsers_UserId",
                table: "Cinemas");

            migrationBuilder.DropIndex(
                name: "IX_Cinemas_UserId",
                table: "Cinemas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Cinemas");
        }
    }
}
