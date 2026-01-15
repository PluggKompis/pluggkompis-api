using Application.Common.Interfaces;
using Application.TimeSlots.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;

namespace Application.TimeSlots.Queries.GetTimeSlotsByVenue
{
    /// <summary>
    /// Handles retrieving all TimeSlots for a venue
    /// </summary>
    public class GetTimeSlotsByVenueQueryHandler : IRequestHandler<GetTimeSlotsByVenueQuery, OperationResult<List<TimeSlotDto>>>
    {
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;

        public GetTimeSlotsByVenueQueryHandler(ITimeSlotRepository timeSlotRepository, IVenueRepository venueRepository, IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository;
            _venueRepository = venueRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<TimeSlotDto>>> Handle(GetTimeSlotsByVenueQuery request, CancellationToken cancellationToken)
        {
            // Verify venue exists
            var venue = await _venueRepository.GetByIdAsync(request.VenueId);
            if (venue == null)
            {
                return OperationResult<List<TimeSlotDto>>.Failure("Venue not found.");
            }

            // Get all timeslots for the venue
            var timeSlots = await _timeSlotRepository.GetByVenueIdAsync(request.VenueId);

            // Filter out cancelled if needed
            if (!request.IncludeCancelled)
            {
                timeSlots = timeSlots
                    .Where(ts => ts.Status != TimeSlotStatus.Cancelled)
                    .ToList();
            }

            // Map to DTOs
            var dtos = _mapper.Map<List<TimeSlotDto>>(timeSlots);

            return OperationResult<List<TimeSlotDto>>.Success(dtos);
        }
    }
}
