using Application.Auth.Dtos;
using AutoMapper;
using Domain.Models.Entities.Users;

namespace Application.Auth.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterUserDto, User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshTokenExpiresAt, opt => opt.Ignore());

            CreateMap<User, UserDtoResponse>()
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));
        }
    }
}
