namespace Application.VolunteerShifts.Dtos
{
    public class MarkAttendanceRequest
    {
        public bool IsAttended { get; set; }
        public string? Notes { get; set; }
    }
}
