using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using Application.Volunteers.Mapping;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Volunteers;
using MediatR;

namespace Application.Volunteers.Commands.CreateMyVolunteerProfile
{
    public class CreateMyVolunteerProfileCommandHandler
        : IRequestHandler<CreateMyVolunteerProfileCommand, OperationResult<VolunteerProfileDto>>
    {
        private readonly IVolunteerProfileRepository _profiles;
        private readonly IVolunteerSubjectRepository _subjects;
        private readonly IMapper _mapper;

        public CreateMyVolunteerProfileCommandHandler(
            IVolunteerProfileRepository profiles,
            IVolunteerSubjectRepository subjects,
            IMapper mapper)
        {
            _profiles = profiles;
            _subjects = subjects;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerProfileDto>> Handle(
            CreateMyVolunteerProfileCommand request,
            CancellationToken cancellationToken)
        {
            var existing = await _profiles.GetByVolunteerIdAsync(request.VolunteerId);
            if (existing is not null)
                return OperationResult<VolunteerProfileDto>.Failure("Volunteer profile already exists.");

            var profile = new VolunteerProfile
            {
                VolunteerId = request.VolunteerId,
                Bio = request.Dto.Bio,
                Experience = request.Dto.Experience,
                MaxHoursPerWeek = request.Dto.MaxHoursPerWeek,
                PreferredVenueId = request.Dto.PreferredVenueId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _profiles.UpsertAsync(profile);

            var subjectRows = request.Dto.Subjects.Select(s => new VolunteerSubject
            {
                VolunteerId = request.VolunteerId,
                SubjectId = s.SubjectId,
                ConfidenceLevel = s.ConfidenceLevel
            });

            await _subjects.ReplaceVolunteerSubjectsAsync(request.VolunteerId, subjectRows);

            return await VolunteerProfileMapper.MapAsync(profile, _subjects, _mapper);
        }
    }
}
