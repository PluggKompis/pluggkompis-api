using Application.Volunteers.Queries.GetAvailableShifts.Models;
using Domain.Models.Common;

namespace Application.Common.Interfaces
{
    public interface IAvailableShiftsRepository
    {
        Task<OperationResult<List<AvailableShiftModel>>> GetAvailableShiftsAsync(
            Guid volunteerId,
            CancellationToken ct);
    }
}
