using Application.Venues.Dtos;
using Domain.Models.Entities.Venues;

namespace Application.Common.Interfaces
{
    public interface IVenueRepository
    {
        Task<List<Venue>> GetAllAsync(VenueFilterParams filters);
        Task<int> GetCountAsync(VenueFilterParams filters);
        Task<Venue?> GetByIdAsync(Guid id);
        Task<Venue?> GetByIdWithDetailsAsync(Guid id);
        Task<Venue?> GetByCoordinatorIdAsync(Guid coordinatorId);
        Task<Venue> CreateAsync(Venue venue);
        Task UpdateAsync(Venue venue);
        Task DeleteAsync(Guid id);
    }
}
