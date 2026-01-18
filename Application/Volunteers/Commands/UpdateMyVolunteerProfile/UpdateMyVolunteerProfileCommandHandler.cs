using Application.Volunteers.Dtos;
using Application.Volunteers.Services;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Commands.UpdateMyVolunteerProfile
{
    public class UpdateMyVolunteerProfileCommandHandler
        : IRequestHandler<UpdateMyVolunteerProfileCommand, OperationResult<VolunteerProfileDto>>
    {
        private readonly IVolunteerProfileService _service;

        public UpdateMyVolunteerProfileCommandHandler(IVolunteerProfileService service)
        {
            _service = service;
        }

        public Task<OperationResult<VolunteerProfileDto>> Handle(UpdateMyVolunteerProfileCommand request, CancellationToken cancellationToken)
            => _service.PatchMyProfileAsync(request.VolunteerId, request.Dto);
    }
}
