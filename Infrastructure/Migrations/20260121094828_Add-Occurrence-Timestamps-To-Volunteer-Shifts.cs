using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOccurrenceTimestampsToVolunteerShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OccurrenceEndUtc",
                table: "VolunteerShifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OccurrenceStartUtc",
                table: "VolunteerShifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerShifts_VolunteerId_OccurrenceStartUtc",
                table: "VolunteerShifts",
                columns: new[] { "VolunteerId", "OccurrenceStartUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VolunteerShifts_VolunteerId_OccurrenceStartUtc",
                table: "VolunteerShifts");

            migrationBuilder.DropColumn(
                name: "OccurrenceEndUtc",
                table: "VolunteerShifts");

            migrationBuilder.DropColumn(
                name: "OccurrenceStartUtc",
                table: "VolunteerShifts");
        }
    }
}
