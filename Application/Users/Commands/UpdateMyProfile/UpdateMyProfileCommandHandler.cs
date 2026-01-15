using Application.Common.Interfaces;
using Application.Users.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var newEmail = request.Dto.Email.Trim();

            // if changing email, ensure it's not used by someone else
            if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _authRepo.EmailExistsAsync(newEmail, cancellationToken);
                if (exists)
                    return OperationResult<MyProfileDto>.Failure("Email is already in use");
            }

            user.FirstName = request.Dto.FirstName.Trim();
            user.LastName = request.Dto.LastName.Trim();
            user.Email = newEmail;

            await _users.UpdateAsync(user);

            return OperationResult<MyProfileDto>.Success(_mapper.Map<MyProfileDto>(user));
        }
    }
}
