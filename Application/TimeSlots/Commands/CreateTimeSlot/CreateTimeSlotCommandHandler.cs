using Application.Common.Interfaces;
using Application.TimeSlots.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;
using MediatR;

namespace Application.TimeSlots.Commands.CreateTimeSlot
{
    /// <summary>
    /// Handles TimeSlot creation with validation and authorization
    /// </summary>
    public class CreateTimeSlotCommandHandler : IRequestHandler<CreateTimeSlotCommand, OperationResult<TimeSlotDto>>
    {
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;

        public CreateTimeSlotCommandHandler(
            ITimeSlotRepository timeSlotRepository,
            IVenueRepository venueRepository,
            IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository;
            _venueRepository = venueRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<TimeSlotDto>> Handle(CreateTimeSlotCommand command, CancellationToken cancellationToken)
        {
            // Verify Venue exists
            var venue = await _venueRepository.GetByIdAsync(command.Request.VenueId);
            if (venue == null)
            {
                return OperationResult<TimeSlotDto>.Failure("Venue not found.");
            }

            // Autorization: Only venue coordinator can create TimeSlots for it
            if (venue.CoordinatorId != command.CoordinatorId)
            {
                return OperationResult<TimeSlotDto>.Failure("You can only create timeslots for your own venue.");
            }

            // Business rule: Check for overlapping timeslots
            var hasOverlap = await _timeSlotRepository.HasOverlappingTimeSlotAsync(
                command.Request.VenueId,
                command.Request.DayOfWeek,
                command.Request.StartTime,
                command.Request.EndTime);

            if (hasOverlap)
            {
               return OperationResult<TimeSlotDto>.Failure($"TimeSlot overlaps with existing timeslot on {command.Request.DayOfWeek}");
            }

            // Create TimeSlot entity
            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                VenueId = command.Request.VenueId,
                DayOfWeek = command.Request.DayOfWeek,
                StartTime = command.Request.StartTime,
                EndTime = command.Request.EndTime,
                MaxStudents = command.Request.MaxStudents,
                IsRecurring = command.Request.IsRecurring,
                SpecificDate = command.Request.SpecificDate,
                Status = TimeSlotStatus.Open,  // Always start as Open
                Subjects = command.Request.SubjectIds.Select(subjectId => new TimeSlotSubject
                {
                    TimeSlotId = Guid.NewGuid(),  // Will be set by TimeSlot.Id
                    SubjectId = subjectId
                }).ToList()
            };

            var createdTimeSlot = await _timeSlotRepository.CreateAsync(timeSlot);

            // Reload with details for mapping
            var timeSlotWithDetails = await _timeSlotRepository.GetByIdWithDetailsAsync(createdTimeSlot.Id);

            // Map to DTO
            var dto = _mapper.Map<TimeSlotDto>(timeSlotWithDetails);

            return OperationResult<TimeSlotDto>.Success(dto);
        }
    }
}
