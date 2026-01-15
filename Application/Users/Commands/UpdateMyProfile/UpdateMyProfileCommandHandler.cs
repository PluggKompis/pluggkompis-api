using Application.Common.Interfaces;
using Application.Users.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;

namespace Application.Users.Commands.UpdateMyProfile
{
    public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, OperationResult<MyProfileDto>>
    {
        private readonly IGenericRepository<User> _users;
        private readonly IAuthRepository _authRepo;
        private readonly IMapper _mapper;

        public UpdateMyProfileCommandHandler(
            IGenericRepository<User> users,
            IAuthRepository authRepo,
            IMapper mapper)
        {
            _users = users;
            _authRepo = authRepo;
            _mapper = mapper;
        }

        public async Task<OperationResult<MyProfileDto>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByIdAsync(request.UserId);

            if (user is null || !user.IsActive)
                return OperationResult<MyProfileDto>.Failure("User not found");

            // FirstName
            if (request.Dto.FirstName is not null)
                user.FirstName = request.Dto.FirstName.Trim();

            // LastName
            if (request.Dto.LastName is not null)
                user.LastName = request.Dto.LastName.Trim();

            // Email
            if (request.Dto.Email is not null)
            {
                var newEmail = request.Dto.Email.Trim();

                if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = await _authRepo.EmailExistsAsync(newEmail, cancellationToken);
                    if (exists)
                        return OperationResult<MyProfileDto>.Failure("Email is already in use");

                    user.Email = newEmail;
                }
            }

            await _users.UpdateAsync(user);

            return OperationResult<MyProfileDto>.Success(_mapper.Map<MyProfileDto>(user));
        }
    }
}
