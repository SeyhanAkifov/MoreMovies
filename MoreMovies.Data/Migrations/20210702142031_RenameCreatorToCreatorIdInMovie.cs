using Microsoft.EntityFrameworkCore.Migrations;

namespace MoreMovies.Data.Migrations
{
    public partial class RenameCreatorToCreatorIdInMovie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Creator",
                table: "Movies",
                newName: "CreatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Movies",
                newName: "Creator");
        }
    }
}
