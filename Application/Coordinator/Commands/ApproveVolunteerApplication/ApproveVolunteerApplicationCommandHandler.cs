using Application.Volunteers.Services;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Commands.ApproveVolunteerApplication
{
    public class ApproveVolunteerApplicationCommandHandler : IRequestHandler<ApproveVolunteerApplicationCommand, OperationResult>
    {
        private readonly IVolunteerService _service;

        public ApproveVolunteerApplicationCommandHandler(IVolunteerService service)
        {
            _service = service;
        }

        public Task<OperationResult> Handle(ApproveVolunteerApplicationCommand request, CancellationToken cancellationToken)
            => _service.ApproveAsync(request.CoordinatorId, request.ApplicationId, request.Dto.DecisionNote);
    }
}
