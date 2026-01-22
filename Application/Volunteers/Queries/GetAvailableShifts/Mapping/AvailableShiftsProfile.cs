using Application.Volunteers.Dtos;
using Application.Volunteers.Queries.GetAvailableShifts.Models;
using AutoMapper;

namespace Application.Volunteers.Queries.GetAvailableShifts.Mapping
{
    public class AvailableShiftsProfile : Profile
    {
        public AvailableShiftsProfile()
        {
            CreateMap<AvailableShiftModel, AvailableShiftDto>()
                .ForMember(d => d.Subjects, opt => opt.MapFrom(s => s.Subjects));
        }
    }
}
