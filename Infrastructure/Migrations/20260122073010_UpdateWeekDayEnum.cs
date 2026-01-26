using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWeekDayEnum : Migration
    {
        // Infrastructure/Data/Migrations/[timestamp]_UpdateWeekDayEnum.cs
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Shift all existing values by +1 (Monday 0→1, Tuesday 1→2, etc.)
            migrationBuilder.Sql(@"
        UPDATE TimeSlots 
        SET DayOfWeek = CASE 
            WHEN DayOfWeek = 6 THEN 0  -- Sunday was 6, now 0
            ELSE DayOfWeek + 1         -- Everything else shifts up
        END
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the shift
            migrationBuilder.Sql(@"
        UPDATE TimeSlots 
        SET DayOfWeek = CASE 
            WHEN DayOfWeek = 0 THEN 6  -- Sunday back to 6
            ELSE DayOfWeek - 1         -- Everything else shifts down
        END
    ");
        }
    }
}
