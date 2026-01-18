using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using Application.Volunteers.Mapping;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.GetMyVolunteerProfile
{
    public class GetMyVolunteerProfileQueryHandler
        : IRequestHandler<GetMyVolunteerProfileQuery, OperationResult<VolunteerProfileDto>>
    {
        private readonly IVolunteerProfileRepository _profiles;
        private readonly IVolunteerSubjectRepository _subjects;
        private readonly IMapper _mapper;

        public GetMyVolunteerProfileQueryHandler(
            IVolunteerProfileRepository profiles,
            IVolunteerSubjectRepository subjects,
            IMapper mapper)
        {
            _profiles = profiles;
            _subjects = subjects;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerProfileDto>> Handle(
            GetMyVolunteerProfileQuery request,
            CancellationToken cancellationToken)
        {
            var profile = await _profiles.GetByVolunteerIdAsync(request.VolunteerId);
            if (profile is null)
                return OperationResult<VolunteerProfileDto>.Failure("Volunteer profile not found.");

            return await VolunteerProfileMapper.MapAsync(profile, _subjects, _mapper);
        }
    }
}
