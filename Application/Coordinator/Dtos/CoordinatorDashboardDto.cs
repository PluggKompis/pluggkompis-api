namespace Application.Coordinator.Dtos
{
    public class CoordinatorDashboardDto
    {
        public int TotalBookingsThisWeek { get; set; }
        public int TotalVolunteers { get; set; }
        public int UnfilledShiftsCount { get; set; }

        public List<SubjectCoverageDto> SubjectCoverage { get; set; } = new();
        public List<UpcomingShiftDto> UpcomingShifts { get; set; } = new();
        public List<VolunteerUtilizationDto> VolunteerUtilization { get; set; } = new();
    }

    public class SubjectCoverageDto
    {
        public string SubjectName { get; set; } = default!;
        public int VolunteersCount { get; set; }
    }

    public class VolunteerUtilizationDto
    {
        public Guid VolunteerId { get; set; }
        public string VolunteerName { get; set; } = default!;
        public double HoursThisWeek { get; set; }
    }

    public class UpcomingShiftDto
    {
        public Guid TimeSlotId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }

        public int BookingsCount { get; set; }
        public int VolunteersCount { get; set; }

        public List<string> VolunteerNames { get; set; } = new();
        public List<string> SubjectNames { get; set; } = new();
    }
}
