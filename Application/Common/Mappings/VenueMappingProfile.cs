using Application.Venues.Dtos;
using AutoMapper;
using Domain.Models.Entities.Venues;

namespace Application.Common.Mappings
{
    public class VenueMappingProfile : Profile
    {
        public VenueMappingProfile()
        {
            // Venue → VenueDto
            CreateMap<Venue, VenueDto>()
                .ForMember(dest => dest.CoordinatorName,
                    opt => opt.MapFrom(src =>
                        src.Coordinator != null
                            ? $"{src.Coordinator.FirstName} {src.Coordinator.LastName}"
                            : "Unknown Coordinator"))  
                .ForMember(dest => dest.AvailableSubjects,
                    opt => opt.MapFrom(src => src.TimeSlots
                        .SelectMany(ts => ts.Subjects)
                        .Select(tss => tss.Subject.Name)
                        .Distinct()
                        .ToList()))
                .ForMember(dest => dest.AvailableDays,
                    opt => opt.MapFrom(src => src.TimeSlots
                        .Select(ts => ts.DayOfWeek.ToString())
                        .Distinct()
                        .ToList()));

            // Venue → VenueDetailDto
            CreateMap<Venue, VenueDetailDto>()
                .ForMember(dest => dest.CoordinatorName,
                    opt => opt.MapFrom(src =>
                        src.Coordinator != null
                            ? $"{src.Coordinator.FirstName} {src.Coordinator.LastName}"
                            : "Unknown Coordinator"));  
        }
    }
}
