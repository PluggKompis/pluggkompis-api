using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addvolunteerApplicationandvolunteerProfileadjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerProfiles_Venues_VenueId",
                table: "VolunteerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerProfiles_IsApproved",
                table: "VolunteerProfiles");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "VolunteerProfiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "VenueId",
                table: "VolunteerProfiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "VolunteerProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VolunteerApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByCoordinatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolunteerApplications_Users_ReviewedByCoordinatorId",
                        column: x => x.ReviewedByCoordinatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VolunteerApplications_Users_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerApplications_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_AppliedAt",
                table: "VolunteerApplications",
                column: "AppliedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_ReviewedByCoordinatorId",
                table: "VolunteerApplications",
                column: "ReviewedByCoordinatorId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_VenueId_Status",
                table: "VolunteerApplications",
                columns: new[] { "VenueId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_VolunteerId_Status",
                table: "VolunteerApplications",
                columns: new[] { "VolunteerId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerProfiles_Venues_VenueId",
                table: "VolunteerProfiles",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerProfiles_Venues_VenueId",
                table: "VolunteerProfiles");

            migrationBuilder.DropTable(
                name: "VolunteerApplications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "VolunteerProfiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "VenueId",
                table: "VolunteerProfiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "VolunteerProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerProfiles_IsApproved",
                table: "VolunteerProfiles",
                column: "IsApproved");

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerProfiles_Venues_VenueId",
                table: "VolunteerProfiles",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
