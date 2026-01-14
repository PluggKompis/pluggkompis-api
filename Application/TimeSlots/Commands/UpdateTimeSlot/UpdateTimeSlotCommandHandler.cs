using Application.Common.Interfaces;
using Application.TimeSlots.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using MediatR;

namespace Application.TimeSlots.Commands.UpdateTimeSlot
{
    /// <summary>
    /// Handles TimeSlot updates with validation and authorization
    /// </summary>
    public class UpdateTimeSlotCommandHandler : IRequestHandler<UpdateTimeSlotCommand, OperationResult<TimeSlotDto>>
    {
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IMapper _mapper;

        public UpdateTimeSlotCommandHandler(ITimeSlotRepository timeSlotRepository, IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<TimeSlotDto>> Handle(UpdateTimeSlotCommand command, CancellationToken cancellationToken)
        {
            // Fetch TimeSlot with Venue details
            var timeSlot = await _timeSlotRepository.GetByIdWithDetailsAsync(command.TimeSlotId);

            if(timeSlot == null)
            {
                return OperationResult<TimeSlotDto>.Failure("TimeSlot not found.");
            }

            // Authorization: Only venue coordinator can update timeslots for their venue
            if (timeSlot.Venue.CoordinatorId != command.CoordinatorId)
            {
                return OperationResult<TimeSlotDto>.Failure("You can only update timeslots from your own Venue.");
            }

            // Business rule: Check overlapping timeslots (excluding current)
            var hasOverlap = await _timeSlotRepository.HasOverlappingTimeSlotAsync(
                timeSlot.VenueId,
                command.Request.DayOfWeek,
                command.Request.StartTime,
                command.Request.EndTime,
                command.TimeSlotId);

            if(hasOverlap)
            {
                return OperationResult<TimeSlotDto>.Failure($"TimeSlot overlaps with existing timeslot on {command.Request.DayOfWeek}");
            }

            // Update properties
            timeSlot.DayOfWeek = command.Request.DayOfWeek;
            timeSlot.StartTime = command.Request.StartTime;
            timeSlot.EndTime = command.Request.EndTime;
            timeSlot.MaxStudents = command.Request.MaxStudents;
            timeSlot.IsRecurring = command.Request.IsRecurring;
            timeSlot.SpecificDate = command.Request.SpecificDate;
            timeSlot.Status = command.Request.Status;

            // Update subjects (remove old, add new)
            timeSlot.Subjects.Clear();
            foreach(var subjectId in command.Request.SubjectIds)
            {
                timeSlot.Subjects.Add(new TimeSlotSubject
                {
                    TimeSlotId = timeSlot.Id,
                    SubjectId = subjectId
                });
            }

            // Save changes
            await _timeSlotRepository.UpdateAsync(timeSlot);

            // Reload with updated details
            var updatedTimeSlot = await _timeSlotRepository.GetByIdWithDetailsAsync(command.TimeSlotId);

            // Map to DTO
            var dto = _mapper.Map<TimeSlotDto>(updatedTimeSlot);

            return OperationResult<TimeSlotDto>.Success(dto);
        }
    }
}
