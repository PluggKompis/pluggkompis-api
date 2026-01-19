using Application.TimeSlots.Dtos;
using Domain.Models.Enums;

namespace Application.Venues.Dtos
{
    /// <summary>
    /// Summary information about a venue for list views
    /// </summary>
    /// 
    public class VenueDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public required string PostalCode { get; set; }
        public required string Description { get; set; }
        public required string ContactEmail { get; set; }
        public required string ContactPhone { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Info of the coordinator managing this venue
        /// </summary>
        public Guid CoordinatorId { get; set; }
        public required string CoordinatorName { get; set; }

        /// <summary>
        /// List of subjects available at this venue (distinct from all timeslots)
        /// </summary>
        public List<string> AvailableSubjects { get; set; } = new();

        /// <summary>
        /// Days of the week when this venue has timeslots
        /// </summary>
        public List<string> AvailableDays { get; set; } = new();
    }

    /// <summary>
    /// Detailed venue information including timeslots and volunteers
    /// </summary>
    public class VenueDetailDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public required string PostalCode { get; set; }
        public required string Description { get; set; }
        public required string ContactEmail { get; set; }
        public required string ContactPhone { get; set; }
        public bool IsActive { get; set; }

        public Guid CoordinatorId { get; set; }
        public required string CoordinatorName { get; set; }

        /// <summary>
        /// Schedule of available homework help sessions
        /// </summary>
        public List<TimeSlotDto> TimeSlots { get; set; } = new();

        /// <summary>
        /// Approved volunteers working at this venue
        /// </summary>
        public List<VolunteerSummaryDto> Volunteers { get; set; } = new();
    }

    // Remove the placeholder below when Voluteer features are implemented
    public class VolunteerSummaryDto
    {
        public Guid VolunteerId { get; set; }
        public required string VolunteerName { get; set; }
        public List<string> Subjects { get; set; } = new();
    }

    /// <summary>
    /// Request to create a new venue
    /// </summary>
    public class CreateVenueRequest
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Description { get; set; }
        public required string ContactEmail { get; set; }
        public required string ContactPhone { get; set; }
    }

    /// <summary>
    /// Request to update an existing venue
    /// </summary>
    public class UpdateVenueRequest
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Description { get; set; }
        public required string ContactEmail { get; set; }
        public required string ContactPhone { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Filter parameters for venue search
    /// </summary>
    public class VenueFilterParams
    {
        /// <summary>
        /// Filter by city name (exact match)
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Filter venues that offer at least one of these subjects
        /// </summary>
        public List<Guid>? SubjectIds { get; set; }

        /// <summary>
        /// Filter venues that have timeslots on at least one of these days
        /// </summary>
        public List<WeekDay>? DaysOfWeek { get; set; }

        /// <summary>
        /// Filter by active/inactive status. Defaults to true (only active venues)
        /// </summary>
        public bool? IsActive { get; set; } = true;

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
