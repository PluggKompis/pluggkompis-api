using Application.VolunteerShifts.Dtos;
using AutoMapper;
using Domain.Models.Entities.Volunteers;

namespace Application.VolunteerShifts.Mapping
{
    public class VolunteerShiftProfile : Profile
    {
        public VolunteerShiftProfile()
        {
            CreateMap<VolunteerShift, VolunteerShiftDto>()
                .ForMember(d => d.VenueId, opt => opt.MapFrom(s => s.TimeSlot.VenueId))
                .ForMember(d => d.VenueName, opt => opt.MapFrom(s => s.TimeSlot.Venue.Name))
                .ForMember(d => d.IsRecurring, opt => opt.MapFrom(s => s.TimeSlot.IsRecurring))
                .ForMember(d => d.SpecificDate, opt => opt.MapFrom(s => s.TimeSlot.SpecificDate))
                .ForMember(d => d.DayOfWeek, opt => opt.MapFrom(s => s.TimeSlot.DayOfWeek))
                .ForMember(d => d.StartTime, opt => opt.MapFrom(s => s.TimeSlot.StartTime))
                .ForMember(d => d.EndTime, opt => opt.MapFrom(s => s.TimeSlot.EndTime));
        }
    }
}
