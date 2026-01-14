using Application.Children.Dtos;
using AutoMapper;
using Domain.Models.Entities.Children;

namespace Application.Children.Mapping
{
    public class ChildProfile : Profile
    {
        public ChildProfile()
        {
            CreateMap<Child, ChildDto>();

            CreateMap<CreateChildRequest, Child>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ParentId, opt => opt.Ignore())
                .ForMember(d => d.Parent, opt => opt.Ignore())
                .ForMember(d => d.Bookings, opt => opt.Ignore());

            CreateMap<UpdateChildRequest, Child>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ParentId, opt => opt.Ignore())
                .ForMember(d => d.Parent, opt => opt.Ignore())
                .ForMember(d => d.Bookings, opt => opt.Ignore());
        }
    }
}
