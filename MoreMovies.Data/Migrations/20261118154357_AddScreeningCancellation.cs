using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoreMovies.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScreeningCancellation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancelledAt",
                table: "CinemaPojections",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "CinemaPojections");
        }
    }
}
