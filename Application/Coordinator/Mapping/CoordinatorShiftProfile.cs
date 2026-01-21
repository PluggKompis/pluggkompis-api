using Application.Coordinator.Dtos;
using AutoMapper;
using Domain.Models.Entities.Volunteers;

namespace Application.Coordinator.Mapping
{
    public class CoordinatorShiftProfile : Profile
    {
        public CoordinatorShiftProfile()
        {
            CreateMap<VolunteerShift, CoordinatorShiftDto>()
                .ForMember(d => d.ShiftId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.VolunteerName, opt => opt.MapFrom(s => $"{s.Volunteer.FirstName} {s.Volunteer.LastName}"))
                .ForMember(d => d.VenueId, opt => opt.MapFrom(s => s.TimeSlot.VenueId))
                .ForMember(d => d.VenueName, opt => opt.MapFrom(s => s.TimeSlot.Venue.Name))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        }
    }
}
