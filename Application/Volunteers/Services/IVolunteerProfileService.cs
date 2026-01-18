using Application.Volunteers.Dtos;
using Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Volunteers.Services
{
    public interface IVolunteerProfileService
    {
        Task<OperationResult<VolunteerProfileDto>> GetMyProfileAsync(Guid volunteerId);
        Task<OperationResult<VolunteerProfileDto>> CreateMyProfileAsync(Guid volunteerId, CreateVolunteerProfileRequest dto);
        Task<OperationResult<VolunteerProfileDto>> PatchMyProfileAsync(Guid volunteerId, UpdateVolunteerProfileRequest dto);
    }
}
