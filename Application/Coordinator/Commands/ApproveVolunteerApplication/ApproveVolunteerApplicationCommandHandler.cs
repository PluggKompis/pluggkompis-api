using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;

namespace Application.Coordinator.Commands.ApproveVolunteerApplication
{
    public class ApproveVolunteerApplicationCommandHandler
        : IRequestHandler<ApproveVolunteerApplicationCommand, OperationResult>
    {
        private readonly IVolunteerApplicationRepository _applications;

        public ApproveVolunteerApplicationCommandHandler(IVolunteerApplicationRepository applications)
        {
            _applications = applications;
        }

        public async Task<OperationResult> Handle(
            ApproveVolunteerApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var application = await _applications.GetByIdAsync(request.ApplicationId);
            if (application is null)
                return OperationResult.Failure("Application not found.");

            if (application.Venue.CoordinatorId != request.CoordinatorId)
                return OperationResult.Failure("Forbidden: you can only manage applications for your own venues.");

            if (application.Status != VolunteerApplicationStatus.Pending)
                return OperationResult.Failure("Only pending applications can be approved.");

            if (await _applications.HasApplicationWithStatusAsync(application.VolunteerId, VolunteerApplicationStatus.Approved))
                return OperationResult.Failure("This volunteer is already approved at another venue.");

            application.Status = VolunteerApplicationStatus.Approved;
            application.ReviewedByCoordinatorId = request.CoordinatorId;
            application.ReviewedAt = DateTime.UtcNow;
            application.DecisionNote = request.Dto.DecisionNote;

            await _applications.UpdateAsync(application);

            return OperationResult.Success();
        }
    }
}
