using Application.Common.Interfaces;
using Application.Users.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;

namespace Application.Users.Queries.GetMyProfile
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, OperationResult<MyProfileDto>>
    {
        private readonly IGenericRepository<User> _users;
        private readonly IMapper _mapper;

        public GetMyProfileQueryHandler(
            IGenericRepository<User> users,
            IMapper mapper)
        {
            _users = users;
            _mapper = mapper;
        }

        public async Task<OperationResult<MyProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByIdAsync(request.UserId);

            if (user is null || !user.IsActive)
                return OperationResult<MyProfileDto>.Failure("User not found");

            return OperationResult<MyProfileDto>.Success(_mapper.Map<MyProfileDto>(user));
        }
    }
}
