using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoreMovies.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTestTicketConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ConfirmedAt",
                table: "ScreeningSeatHolds",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "ScreeningSeatHolds");
        }
    }
}
