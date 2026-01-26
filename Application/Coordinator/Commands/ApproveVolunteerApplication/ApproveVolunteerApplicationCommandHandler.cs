using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;

namespace Application.Coordinator.Commands.ApproveVolunteerApplication
{
    public class ApproveVolunteerApplicationCommandHandler
        : IRequestHandler<ApproveVolunteerApplicationCommand, OperationResult<VolunteerApplicationDto>>
    {
        private readonly IVolunteerApplicationRepository _applications;
        private readonly IMapper _mapper;

        public ApproveVolunteerApplicationCommandHandler(
            IVolunteerApplicationRepository applications,
            IMapper mapper)
        {
            _applications = applications;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerApplicationDto>> Handle(
            ApproveVolunteerApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var application = await _applications.GetByIdAsync(request.ApplicationId);
            if (application is null)
                return OperationResult<VolunteerApplicationDto>.Failure("Application not found.");

            if (application.Venue.CoordinatorId != request.CoordinatorId)
                return OperationResult<VolunteerApplicationDto>.Failure(
                    "Forbidden: you can only manage applications for your own venues.");

            if (application.Status != VolunteerApplicationStatus.Pending)
                return OperationResult<VolunteerApplicationDto>.Failure(
                    "Only pending applications can be approved.");

            //if (await _applications.HasApplicationWithStatusAsync(
            //        application.VolunteerId,
            //        VolunteerApplicationStatus.Approved))
            //    return OperationResult<VolunteerApplicationDto>.Failure(
            //        "This volunteer is already approved at another venue.");

            application.Status = VolunteerApplicationStatus.Approved;
            application.ReviewedByCoordinatorId = request.CoordinatorId;
            application.ReviewedAt = DateTime.UtcNow;
            application.DecisionNote = request.Dto.DecisionNote;

            await _applications.UpdateAsync(application);

            var dto = _mapper.Map<VolunteerApplicationDto>(application);
            return OperationResult<VolunteerApplicationDto>.Success(dto);
        }
    }
}
