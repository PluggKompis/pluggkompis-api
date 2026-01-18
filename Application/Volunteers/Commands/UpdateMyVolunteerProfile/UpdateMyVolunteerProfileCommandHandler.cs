using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using Application.Volunteers.Mapping;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using MediatR;

namespace Application.Volunteers.Commands.UpdateMyVolunteerProfile
{
    public class UpdateMyVolunteerProfileCommandHandler
        : IRequestHandler<UpdateMyVolunteerProfileCommand, OperationResult<VolunteerProfileDto>>
    {
        private readonly IVolunteerProfileRepository _profiles;
        private readonly IVolunteerSubjectRepository _subjects;
        private readonly IMapper _mapper;

        public UpdateMyVolunteerProfileCommandHandler(
            IVolunteerProfileRepository profiles,
            IVolunteerSubjectRepository subjects,
            IMapper mapper)
        {
            _profiles = profiles;
            _subjects = subjects;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerProfileDto>> Handle(
            UpdateMyVolunteerProfileCommand request,
            CancellationToken cancellationToken)
        {
            var profile = await _profiles.GetByVolunteerIdAsync(request.VolunteerId);
            if (profile is null)
                return OperationResult<VolunteerProfileDto>.Failure("Volunteer profile not found.");

            // PATCH semantics
            if (request.Dto.Bio is not null)
                profile.Bio = request.Dto.Bio;

            if (request.Dto.Experience is not null)
                profile.Experience = request.Dto.Experience;

            if (request.Dto.MaxHoursPerWeek.HasValue)
                profile.MaxHoursPerWeek = request.Dto.MaxHoursPerWeek.Value;

            if (request.Dto.PreferredVenueId.HasValue)
                profile.PreferredVenueId = request.Dto.PreferredVenueId.Value;

            profile.UpdatedAt = DateTime.UtcNow;

            await _profiles.UpsertAsync(profile);

            // Subjects only if provided (null = ignore)
            if (request.Dto.Subjects is not null)
            {
                var subjectRows = request.Dto.Subjects.Select(s => new VolunteerSubject
                {
                    VolunteerId = request.VolunteerId,
                    SubjectId = s.SubjectId,
                    ConfidenceLevel = s.ConfidenceLevel
                });

                await _subjects.ReplaceVolunteerSubjectsAsync(request.VolunteerId, subjectRows);
            }

            return await VolunteerProfileMapper.MapAsync(profile, _subjects, _mapper);
        }
    }
}
