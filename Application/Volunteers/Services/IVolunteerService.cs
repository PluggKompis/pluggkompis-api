using Application.Volunteers.Dtos;
using Domain.Models.Common;

namespace Application.Volunteers.Services
{
    public interface IVolunteerService
    {
        Task<OperationResult<VolunteerProfileDto>> ApplyAsync(Guid volunteerId, ApplyToVenueRequest dto);

        Task<OperationResult<List<VolunteerApplicationDto>>> GetPendingApplicationsForCoordinatorAsync(Guid coordinatorId);

        Task<OperationResult> ApproveAsync(Guid coordinatorId, Guid applicationId, string? note);
        Task<OperationResult> DeclineAsync(Guid coordinatorId, Guid applicationId, string? note);

        Task<OperationResult<List<VolunteerProfileDto>>> GetApprovedVolunteersForVenueAsync(Guid venueId);

    }
}
