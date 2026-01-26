using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Volunteers;

namespace Application.Volunteers.Mapping
{
    internal static class VolunteerProfileMapper
    {
        internal static async Task<OperationResult<VolunteerProfileDto>> MapAsync(
            VolunteerProfile profile,
            IVolunteerSubjectRepository volunteerSubjects,
            IMapper mapper)
        {
            var dto = mapper.Map<VolunteerProfileDto>(profile);

            var subjectRows = await volunteerSubjects.GetVolunteerSubjectsAsync(profile.VolunteerId);

            dto.Subjects = mapper.Map<List<VolunteerSubjectDto>>(subjectRows);

            return OperationResult<VolunteerProfileDto>.Success(dto);
        }
    }
}
