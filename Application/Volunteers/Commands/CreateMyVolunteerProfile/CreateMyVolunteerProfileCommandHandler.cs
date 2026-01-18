using Application.Volunteers.Dtos;
using Application.Volunteers.Services;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Commands.CreateMyVolunteerProfile
{
    public class CreateMyVolunteerProfileCommandHandler
        : IRequestHandler<CreateMyVolunteerProfileCommand, OperationResult<VolunteerProfileDto>>
    {
        private readonly IVolunteerProfileService _service;

        public CreateMyVolunteerProfileCommandHandler(IVolunteerProfileService service)
        {
            _service = service;
        }

        public Task<OperationResult<VolunteerProfileDto>> Handle(CreateMyVolunteerProfileCommand request, CancellationToken cancellationToken)
            => _service.CreateMyProfileAsync(request.VolunteerId, request.Dto);
    }
}
