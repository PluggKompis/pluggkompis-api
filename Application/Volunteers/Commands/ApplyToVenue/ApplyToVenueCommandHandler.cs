using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;
using MediatR;

namespace Application.Volunteers.Commands.ApplyToVenue
{
    public class ApplyToVenueCommandHandler
    : IRequestHandler<ApplyToVenueCommand, OperationResult<VolunteerProfileDto>>
    {
        private readonly IVolunteerProfileRepository _profiles;
        private readonly IVolunteerSubjectRepository _subjects;
        private readonly IVolunteerApplicationRepository _applications;
        private readonly IMapper _mapper;

        public ApplyToVenueCommandHandler(
            IVolunteerProfileRepository profiles,
            IVolunteerSubjectRepository subjects,
            IVolunteerApplicationRepository applications,
            IMapper mapper)
        {
            _profiles = profiles;
            _subjects = subjects;
            _applications = applications;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerProfileDto>> Handle(
            ApplyToVenueCommand request,
            CancellationToken cancellationToken)
        {
            var volunteerId = request.VolunteerId;
            var venueId = request.Dto.VenueId;

            // Must have a profile first (since apply no longer carries profile data)
            var profile = await _profiles.GetByVolunteerIdAsync(volunteerId);
            if (profile is null)
                return OperationResult<VolunteerProfileDto>.Failure("You must create your volunteer profile before applying to a venue.");

            // Business rules
            if (await _applications.HasPendingApplicationForVenueAsync(volunteerId, venueId))
                return OperationResult<VolunteerProfileDto>.Failure("You already have a pending application for this venue.");

            // Create application
            var application = new VolunteerApplication
            {
                Id = Guid.NewGuid(),
                VolunteerId = volunteerId,
                VenueId = venueId,
                Status = VolunteerApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            await _applications.AddAsync(application);

            // Return the volunteer profile snapshot (from stored profile + stored subjects)
            var profileDto = _mapper.Map<VolunteerProfileDto>(profile);

            var volunteerSubjects = await _subjects.GetVolunteerSubjectsAsync(volunteerId);
            profileDto.Subjects = _mapper.Map<List<VolunteerSubjectDto>>(volunteerSubjects);

            return OperationResult<VolunteerProfileDto>.Success(profileDto);
        }
    }
}
