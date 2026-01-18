using Application.Common.Interfaces;
using Application.Subjects.Dtos;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Queries.GetVenueVolunteers
{
    public class GetVenueVolunteersQueryHandler
        : IRequestHandler<GetVenueVolunteersQuery, OperationResult<List<VolunteerProfileDto>>>
    {
        private readonly IVolunteerApplicationRepository _applications;
        private readonly IVolunteerProfileRepository _profiles;
        private readonly IVolunteerSubjectRepository _subjects;
        private readonly IMapper _mapper;

        public GetVenueVolunteersQueryHandler(
            IVolunteerApplicationRepository applications,
            IVolunteerProfileRepository profiles,
            IVolunteerSubjectRepository subjects,
            IMapper mapper)
        {
            _applications = applications;
            _profiles = profiles;
            _subjects = subjects;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<VolunteerProfileDto>>> Handle(
            GetVenueVolunteersQuery request,
            CancellationToken cancellationToken)
        {
            var volunteerIds = await _applications.GetApprovedVolunteerIdsForVenueAsync(request.VenueId);

            if (volunteerIds.Count == 0)
                return OperationResult<List<VolunteerProfileDto>>.Success(new List<VolunteerProfileDto>());

            var profiles = new List<VolunteerProfileDto>();

            foreach (var id in volunteerIds)
            {
                var profile = await _profiles.GetByVolunteerIdAsync(id);
                if (profile is null) continue;

                profiles.Add(_mapper.Map<VolunteerProfileDto>(profile));
            }

            var rawSubjects = await _subjects.GetVolunteerSubjectsAsync(volunteerIds);

            foreach (var p in profiles)
            {
                p.Subjects = rawSubjects
                    .Where(s => s.VolunteerId == p.VolunteerId)
                    .Select(s => new VolunteerSubjectDto
                    {
                        Subject = new SubjectDto
                        {
                            Id = s.Subject.Id,
                            Name = s.Subject.Name,
                            Icon = s.Subject.Icon
                        },
                        ConfidenceLevel = s.ConfidenceLevel
                    })
                    .ToList();
            }

            return OperationResult<List<VolunteerProfileDto>>.Success(profiles);
        }
    }
}
