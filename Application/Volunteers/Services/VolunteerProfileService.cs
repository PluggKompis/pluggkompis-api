using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Volunteers;

namespace Application.Volunteers.Services
{
    public class VolunteerProfileService : IVolunteerProfileService
    {
        private readonly IVolunteerProfileRepository _profiles;
        private readonly IVolunteerSubjectRepository _volunteerSubjects;
        private readonly IMapper _mapper;

        public VolunteerProfileService(
            IVolunteerProfileRepository profiles,
            IVolunteerSubjectRepository volunteerSubjects,
            IMapper mapper)
        {
            _profiles = profiles;
            _volunteerSubjects = volunteerSubjects;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerProfileDto>> GetMyProfileAsync(Guid volunteerId)
        {
            var profile = await _profiles.GetByVolunteerIdAsync(volunteerId);
            if (profile is null)
                return OperationResult<VolunteerProfileDto>.Failure("Volunteer profile not found.");

            return await MapProfileAsync(profile);
        }

        public async Task<OperationResult<VolunteerProfileDto>> CreateMyProfileAsync(Guid volunteerId, CreateVolunteerProfileRequest dto)
        {
            var existing = await _profiles.GetByVolunteerIdAsync(volunteerId);
            if (existing is not null)
                return OperationResult<VolunteerProfileDto>.Failure("Volunteer profile already exists.");

            var profile = new VolunteerProfile
            {
                VolunteerId = volunteerId,
                Bio = dto.Bio,
                Experience = dto.Experience,
                MaxHoursPerWeek = dto.MaxHoursPerWeek,
                PreferredVenueId = dto.PreferredVenueId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _profiles.UpsertAsync(profile);

            // Create subjects (replace semantics still fine on create)
            var subjectRows = dto.Subjects.Select(s => new VolunteerSubject
            {
                VolunteerId = volunteerId,
                SubjectId = s.SubjectId,
                ConfidenceLevel = s.ConfidenceLevel
            });

            await _volunteerSubjects.ReplaceVolunteerSubjectsAsync(volunteerId, subjectRows);

            return await MapProfileAsync(profile);
        }

        public async Task<OperationResult<VolunteerProfileDto>> PatchMyProfileAsync(Guid volunteerId, UpdateVolunteerProfileRequest dto)
        {
            var profile = await _profiles.GetByVolunteerIdAsync(volunteerId);
            if (profile is null)
                return OperationResult<VolunteerProfileDto>.Failure("Volunteer profile not found.");

            // PATCH semantics: update only provided fields
            if (dto.Bio is not null)
                profile.Bio = dto.Bio;

            if (dto.Experience is not null)
                profile.Experience = dto.Experience;

            if (dto.MaxHoursPerWeek.HasValue)
                profile.MaxHoursPerWeek = dto.MaxHoursPerWeek.Value;

            if (dto.PreferredVenueId.HasValue)
                profile.PreferredVenueId = dto.PreferredVenueId.Value;

            profile.UpdatedAt = DateTime.UtcNow;

            await _profiles.UpsertAsync(profile);

            // Subjects: only touch if provided
            if (dto.Subjects is not null)
            {
                var subjectRows = dto.Subjects.Select(s => new VolunteerSubject
                {
                    VolunteerId = volunteerId,
                    SubjectId = s.SubjectId,
                    ConfidenceLevel = s.ConfidenceLevel
                });

                await _volunteerSubjects.ReplaceVolunteerSubjectsAsync(volunteerId, subjectRows);
            }

            return await MapProfileAsync(profile);
        }

        private async Task<OperationResult<VolunteerProfileDto>> MapProfileAsync(VolunteerProfile profile)
        {
            var dto = _mapper.Map<VolunteerProfileDto>(profile);

            var subjectRows = await _volunteerSubjects.GetVolunteerSubjectsAsync(profile.VolunteerId);

            dto.Subjects = _mapper.Map<List<VolunteerSubjectDto>>(subjectRows);

            return OperationResult<VolunteerProfileDto>.Success(dto);
        }
    }
}
