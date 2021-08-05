using Microsoft.EntityFrameworkCore.Migrations;

namespace MoreMovies.Data.Migrations
{
    public partial class RanameCinemaHallIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaPojections_CinemaHalls_CinemaHallId",
                table: "CinemaPojections");

            migrationBuilder.DropColumn(
                name: "CinameHallId",
                table: "CinemaPojections");

            migrationBuilder.AlterColumn<int>(
                name: "CinemaHallId",
                table: "CinemaPojections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaPojections_CinemaHalls_CinemaHallId",
                table: "CinemaPojections",
                column: "CinemaHallId",
                principalTable: "CinemaHalls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaPojections_CinemaHalls_CinemaHallId",
                table: "CinemaPojections");

            migrationBuilder.AlterColumn<int>(
                name: "CinemaHallId",
                table: "CinemaPojections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CinameHallId",
                table: "CinemaPojections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaPojections_CinemaHalls_CinemaHallId",
                table: "CinemaPojections",
                column: "CinemaHallId",
                principalTable: "CinemaHalls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
