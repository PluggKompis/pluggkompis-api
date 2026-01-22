using Application.Subjects.Dtos;
using Application.Venues.Dtos;
using AutoMapper;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;

namespace Application.Common.Mappings
{
    public class VenueMappingProfile : Profile
    {
        public VenueMappingProfile()
        {
            CreateMap<CreateVenueRequest, Venue>();
            CreateMap<UpdateVenueRequest, Venue>();
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
                        .Select(tss => new SubjectDto
                        {
                            Id = tss.Subject.Id,
                            Name = tss.Subject.Name,
                            Icon = tss.Subject.Icon
                        })
                        .GroupBy(s => s.Id)
                        .Select(g => g.First())
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
                            : "Unknown Coordinator"))
                .ForMember(dest => dest.TimeSlots,
                    opt => opt.MapFrom(src => src.TimeSlots));
        }
    }
}
