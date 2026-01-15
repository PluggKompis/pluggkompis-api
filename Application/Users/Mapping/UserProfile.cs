using Application.Users.Dtos;
using AutoMapper;
using Domain.Models.Entities.Users;

namespace Application.Users.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, MyProfileDto>()
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));
        }
    }
}
