using Microsoft.EntityFrameworkCore.Migrations;

namespace MoreMovies.Data.Migrations
{
    public partial class CinemaProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CinemaId",
                table: "CinemaPojections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CinemaPojections_CinemaId",
                table: "CinemaPojections",
                column: "CinemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaPojections_Cinemas_CinemaId",
                table: "CinemaPojections",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaPojections_Cinemas_CinemaId",
                table: "CinemaPojections");

            migrationBuilder.DropTable(
                name: "Cinemas");

            migrationBuilder.DropIndex(
                name: "IX_CinemaPojections_CinemaId",
                table: "CinemaPojections");

            migrationBuilder.DropColumn(
                name: "CinemaId",
                table: "CinemaPojections");
        }
    }
}
