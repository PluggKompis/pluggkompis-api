using Application.Volunteers.Services;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Commands.DeclineVolunteerApplication
{
    public class DeclineVolunteerApplicationCommandHandler : IRequestHandler<DeclineVolunteerApplicationCommand, OperationResult>
    {
        private readonly IVolunteerService _service;

        public DeclineVolunteerApplicationCommandHandler(IVolunteerService service)
        {
            _service = service;
        }

        public Task<OperationResult> Handle(DeclineVolunteerApplicationCommand request, CancellationToken cancellationToken)
            => _service.DeclineAsync(request.CoordinatorId, request.ApplicationId, request.Dto.DecisionNote);
    }
}
