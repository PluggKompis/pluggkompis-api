using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
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
            var dto = request.Dto;

            // Business rules
            if (await _applications.HasApplicationWithStatusAsync(volunteerId, VolunteerApplicationStatus.Pending))
                return OperationResult<VolunteerProfileDto>.Failure("You already have a pending application.");

            if (await _applications.HasApplicationWithStatusAsync(volunteerId, VolunteerApplicationStatus.Approved))
                return OperationResult<VolunteerProfileDto>.Failure("You are already approved at a venue. You must leave the current venue before applying to another.");

            if (await _applications.HasPendingApplicationForVenueAsync(volunteerId, dto.VenueId))
                return OperationResult<VolunteerProfileDto>.Failure("You already have a pending application for this venue.");

            // Upsert profile (profile is separate from application)
            var profile = await _profiles.GetByVolunteerIdAsync(volunteerId)
                         ?? new VolunteerProfile
                         {
                             VolunteerId = volunteerId,
                             CreatedAt = DateTime.UtcNow
                         };

            profile.Bio = dto.Bio;
            profile.Experience = dto.Experience;
            profile.MaxHoursPerWeek = dto.MaxHoursPerWeek;
            profile.PreferredVenueId = dto.VenueId;
            profile.UpdatedAt = DateTime.UtcNow;

            await _profiles.UpsertAsync(profile);

            // Replace volunteer subjects
            var subjectRows = dto.Subjects.Select(s => new VolunteerSubject
            {
                VolunteerId = volunteerId,
                SubjectId = s.SubjectId,
                ConfidenceLevel = s.ConfidenceLevel
            });

            await _subjects.ReplaceVolunteerSubjectsAsync(volunteerId, subjectRows);

            // Create application
            var application = new VolunteerApplication
            {
                Id = Guid.NewGuid(),
                VolunteerId = volunteerId,
                VenueId = dto.VenueId,
                Status = VolunteerApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            await _applications.AddAsync(application);

            // Response DTO
            var profileDto = _mapper.Map<VolunteerProfileDto>(profile);
            var volunteerSubjects = await _subjects.GetVolunteerSubjectsAsync(volunteerId);
            profileDto.Subjects = _mapper.Map<List<VolunteerSubjectDto>>(volunteerSubjects);

            return OperationResult<VolunteerProfileDto>.Success(profileDto);
        }
    }
}
