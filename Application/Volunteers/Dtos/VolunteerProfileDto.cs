using Application.Subjects.Dtos;
using Domain.Models.Enums;

namespace Application.Volunteers.Dtos
{
    public class VolunteerProfileDto
    {
        public Guid VolunteerId { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public int? MaxHoursPerWeek { get; set; }

        public Guid? PreferredVenueId { get; set; }

        public List<VolunteerSubjectDto> Subjects { get; set; } = new();
    }

    public class CreateVolunteerProfileRequest
    {
        public string Bio { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public int? MaxHoursPerWeek { get; set; }
        public Guid? PreferredVenueId { get; set; }

        public List<VolunteerSubjectRequest> Subjects { get; set; } = new();
    }
    public class UpdateVolunteerProfileRequest
    {
        public string? Bio { get; set; }
        public string? Experience { get; set; }
        public int? MaxHoursPerWeek { get; set; }
        public Guid? PreferredVenueId { get; set; }

        // If null → do not touch subjects
        // If empty list → clear subjects
        public List<VolunteerSubjectRequest>? Subjects { get; set; }
    }

    public class VolunteerSubjectDto
    {
        public SubjectDto Subject { get; set; } = default!;
        public ConfidenceLevel ConfidenceLevel { get; set; }
    }
}
