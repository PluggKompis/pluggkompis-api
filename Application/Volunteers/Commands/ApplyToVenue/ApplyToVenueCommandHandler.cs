using Application.Volunteers.Dtos;
using Application.Volunteers.Services;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Commands.ApplyToVenue
{
    public class ApplyToVenueCommandHandler : IRequestHandler<ApplyToVenueCommand, OperationResult<VolunteerProfileDto>>
    {
        private readonly IVolunteerService _service;

        public ApplyToVenueCommandHandler(IVolunteerService service)
        {
            _service = service;
        }

        public Task<OperationResult<VolunteerProfileDto>> Handle(ApplyToVenueCommand request, CancellationToken cancellationToken)
            => _service.ApplyAsync(request.VolunteerId, request.Dto);
    }
}
