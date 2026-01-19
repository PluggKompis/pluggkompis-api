using Application.Subjects.Dtos;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Volunteers;

namespace Application.Volunteers.Mapping
{
    public class VolunteerMappingProfile : Profile
    {
        public VolunteerMappingProfile()
        {
            // Volunteer profile (subjects handled separately)
            CreateMap<VolunteerProfile, VolunteerProfileDto>();

            // Subject catalog mapping
            CreateMap<Domain.Models.Entities.Subjects.Subject, SubjectDto>();

            // VolunteerSubject → VolunteerSubjectDto (join entity)
            CreateMap<VolunteerSubject, VolunteerSubjectDto>()
                .ForMember(
                    d => d.Subject,
                    opt => opt.MapFrom(src => src.Subject)
                );

            // Volunteer application (coordinator views)
            CreateMap<VolunteerApplication, VolunteerApplicationDto>()
                .ForMember(d => d.ApplicationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.VolunteerName,
                    opt => opt.MapFrom(src =>
                        $"{src.Volunteer.FirstName} {src.Volunteer.LastName}"))
                .ForMember(d => d.VolunteerEmail,
                    opt => opt.MapFrom(src => src.Volunteer.Email))
                .ForMember(d => d.VenueName,
                    opt => opt.MapFrom(src => src.Venue.Name));
        }
    }
}
