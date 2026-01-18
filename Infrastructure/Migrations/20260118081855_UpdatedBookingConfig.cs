using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBookingConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotId_BookingDate",
                table: "Bookings",
                columns: new[] { "TimeSlotId", "BookingDate" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_StudentOrChild",
                table: "Bookings",
                sql: "((StudentId IS NOT NULL AND ChildId IS NULL) OR (StudentId IS NULL AND ChildId IS NOT NULL))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotId_BookingDate",
                table: "Bookings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_StudentOrChild",
                table: "Bookings");
        }
    }
}
