using Application.Subjects.Dtos;
using AutoMapper;
using Domain.Models.Entities.Subjects;

namespace Application.Common.Mappings
{
    public class SubjectMappingProfile : Profile
    {
        public SubjectMappingProfile()
        {
            CreateMap<Subject, SubjectDto>();
        }
    }
}
