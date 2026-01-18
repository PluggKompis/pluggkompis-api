using Domain.Models.Enums;

namespace Application.Volunteers.Dtos
{
    public class VolunteerApplicationDto
    {
        public Guid ApplicationId { get; set; }
        public Guid VolunteerId { get; set; }
        public string VolunteerName { get; set; } = string.Empty;
        public string VolunteerEmail { get; set; } = string.Empty;

        public Guid VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;

        public VolunteerApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }
    public class ApplyToVenueRequest
    {
        public Guid VenueId { get; set; }
    }

    public class VolunteerSubjectRequest
    {
        public Guid SubjectId { get; set; }
        public ConfidenceLevel ConfidenceLevel { get; set; }
    }
    public class ApproveVolunteerRequest
    {
        public string? DecisionNote { get; set; }
    }
}
