using Application.TimeSlots.Dtos;
using AutoMapper;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;

namespace Application.Common.Mappings
{
    public class TimeSlotMappingProfile : Profile
    {
        public TimeSlotMappingProfile()
        {
            CreateMap<TimeSlot, TimeSlotDto>()
                .ForMember(dest => dest.VenueName,
                    opt => opt.MapFrom(src => src.Venue != null ? src.Venue.Name : "Unknown"))
                .ForMember(dest => dest.Subjects,
                    opt => opt.MapFrom(src => src.Subjects
                        .Select(ts => ts.Subject.Name)
                        .ToList()))
                .ForMember(dest => dest.CurrentBookings,
                    opt => opt.MapFrom(src => src.Bookings
                        .Count(b => b.Status == BookingStatus.Confirmed)))
                .ForMember(dest => dest.AvailableSpots,
                    opt => opt.MapFrom(src => src.MaxStudents - src.Bookings
                        .Count(b => b.Status == BookingStatus.Confirmed)));
        }
    }
}
