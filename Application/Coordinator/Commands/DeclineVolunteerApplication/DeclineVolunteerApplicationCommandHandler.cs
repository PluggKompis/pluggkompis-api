using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;

namespace Application.Coordinator.Commands.DeclineVolunteerApplication
{
    public class DeclineVolunteerApplicationCommandHandler
        : IRequestHandler<DeclineVolunteerApplicationCommand, OperationResult<VolunteerApplicationDto>>
    {
        private readonly IVolunteerApplicationRepository _applications;
        private readonly IMapper _mapper;

        public DeclineVolunteerApplicationCommandHandler(
            IVolunteerApplicationRepository applications,
            IMapper mapper)
        {
            _applications = applications;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerApplicationDto>> Handle(
            DeclineVolunteerApplicationCommand request,
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
                    "Only pending applications can be declined.");

            application.Status = VolunteerApplicationStatus.Declined;
            application.ReviewedByCoordinatorId = request.CoordinatorId;
            application.ReviewedAt = DateTime.UtcNow;
            application.DecisionNote = request.Dto.DecisionNote;

            await _applications.UpdateAsync(application);

            var dto = _mapper.Map<VolunteerApplicationDto>(application);
            return OperationResult<VolunteerApplicationDto>.Success(dto);
        }
    }
}
