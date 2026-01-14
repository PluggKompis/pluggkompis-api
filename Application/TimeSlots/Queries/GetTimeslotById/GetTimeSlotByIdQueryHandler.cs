using Application.Common.Interfaces;
using Application.TimeSlots.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.TimeSlots.Queries.GetTimeslotById
{
    /// <summary>
    /// Handles retrieving a specific TimeSlot with full details
    /// </summary>
    public class GetTimeSlotByIdQueryHandler : IRequestHandler<GetTimeSlotByIdQuery, OperationResult<TimeSlotDto>>
    {
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IMapper _mapper;

        public GetTimeSlotByIdQueryHandler(ITimeSlotRepository timeSlotRepository, IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<TimeSlotDto>> Handle(GetTimeSlotByIdQuery request, CancellationToken cancellationToken)
        {
            // Get timeslot with all details
            var timeSlot = await _timeSlotRepository.GetByIdWithDetailsAsync(request.TimeSlotId);

            if(timeSlot == null)
            {
                return OperationResult<TimeSlotDto>.Failure("TimeSlot not found.");
            }

            var dto = _mapper.Map<TimeSlotDto>(timeSlot);

            return OperationResult<TimeSlotDto>.Success(dto);
        }
    }
}
