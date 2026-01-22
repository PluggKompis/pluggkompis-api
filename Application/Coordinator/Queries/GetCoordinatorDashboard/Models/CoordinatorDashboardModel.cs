namespace Application.Coordinator.Queries.GetCoordinatorDashboard.Models
{
    public class CoordinatorDashboardModel
    {
        public int TotalBookingsThisWeek { get; set; }
        public int TotalVolunteers { get; set; }
        public int UnfilledShiftsCount { get; set; }

        public List<SubjectCoverageModel> SubjectCoverage { get; set; } = new();
        public List<UpcomingShiftModel> UpcomingShifts { get; set; } = new();
        public List<VolunteerUtilizationModel> VolunteerUtilization { get; set; } = new();
    }

    public class SubjectCoverageModel
    {
        public string SubjectName { get; set; } = default!;
        public int VolunteersCount { get; set; }
    }

    public class VolunteerUtilizationModel
    {
        public Guid VolunteerId { get; set; }
        public string VolunteerName { get; set; } = default!;
        public double HoursThisWeek { get; set; }
    }

    public class UpcomingShiftModel
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
