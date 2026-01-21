using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.GetMyVolunteerApplications
{
    public record GetMyVolunteerApplicationsQuery(Guid VolunteerId)
        : IRequest<OperationResult<List<VolunteerApplicationDto>>>;
}
