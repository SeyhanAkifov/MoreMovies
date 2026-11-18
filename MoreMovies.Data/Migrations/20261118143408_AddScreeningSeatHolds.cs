using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoreMovies.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScreeningSeatHolds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeatRows",
                table: "CinemaPojections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeatsPerRow",
                table: "CinemaPojections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ScreeningSeatHolds",
                columns: table => new
                {
                    ScreeningId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreeningSeatHolds", x => new { x.ScreeningId, x.SeatNumber });
                    table.ForeignKey(
                        name: "FK_ScreeningSeatHolds_CinemaPojections_ScreeningId",
                        column: x => x.ScreeningId,
                        principalTable: "CinemaPojections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScreeningSeatHolds");

            migrationBuilder.DropColumn(
                name: "SeatRows",
                table: "CinemaPojections");

            migrationBuilder.DropColumn(
                name: "SeatsPerRow",
                table: "CinemaPojections");
        }
    }
}
