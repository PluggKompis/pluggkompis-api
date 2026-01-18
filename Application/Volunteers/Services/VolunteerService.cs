using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Volunteers.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IVolunteerProfileRepository _profiles;
        private readonly IVolunteerSubjectRepository _subjects;
        private readonly IVolunteerApplicationRepository _applications;
        private readonly IMapper _mapper;

        public VolunteerService(
            IVolunteerProfileRepository profiles,
            IVolunteerSubjectRepository subjects,
            IVolunteerApplicationRepository applications,
            IMapper mapper)
        {
            _profiles = profiles;
            _subjects = subjects;
            _applications = applications;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerProfileDto>> ApplyAsync(Guid volunteerId, ApplyToVenueRequest dto)
        {
            // Business rules
            if (await _applications.HasApplicationWithStatusAsync(volunteerId, VolunteerApplicationStatus.Pending))
                return OperationResult<VolunteerProfileDto>.Failure("You already have a pending application.");

            if (await _applications.HasApplicationWithStatusAsync(volunteerId, VolunteerApplicationStatus.Approved))
                return OperationResult<VolunteerProfileDto>.Failure("You are already approved at a venue. You must leave the current venue before applying to another.");

            if (await _applications.HasPendingApplicationForVenueAsync(volunteerId, dto.VenueId))
                return OperationResult<VolunteerProfileDto>.Failure("You already have a pending application for this venue.");

            // Upsert profile (profile is separate from application)
            var profile = await _profiles.GetByVolunteerIdAsync(volunteerId)
                          ?? new VolunteerProfile { VolunteerId = volunteerId };

            profile.Bio = dto.Bio;
            profile.Experience = dto.Experience;
            profile.MaxHoursPerWeek = dto.MaxHoursPerWeek;
            profile.PreferredVenueId = dto.VenueId;
            profile.UpdatedAt = DateTime.UtcNow;

            await _profiles.UpsertAsync(profile);

            // Replace volunteer subjects
            var subjectRows = dto.Subjects.Select(s => new VolunteerSubject
            {
                VolunteerId = volunteerId,
                SubjectId = s.SubjectId,
                ConfidenceLevel = s.ConfidenceLevel
            });

            await _subjects.ReplaceVolunteerSubjectsAsync(volunteerId, subjectRows);

            // Create application
            var application = new VolunteerApplication
            {
                Id = Guid.NewGuid(),
                VolunteerId = volunteerId,
                VenueId = dto.VenueId,
                Status = VolunteerApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            await _applications.AddAsync(application);

            // Map response
            var profileDto = _mapper.Map<VolunteerProfileDto>(profile);
            var volunteerSubjects = await _subjects.GetVolunteerSubjectsAsync(volunteerId);
            profileDto.Subjects = _mapper.Map<List<VolunteerSubjectDto>>(volunteerSubjects);

            return OperationResult<VolunteerProfileDto>.Success(profileDto);
        }

        public async Task<OperationResult<List<VolunteerApplicationDto>>> GetPendingApplicationsForCoordinatorAsync(Guid coordinatorId)
        {
            var apps = await _applications.GetPendingForCoordinatorAsync(coordinatorId);
            var dtos = _mapper.Map<List<VolunteerApplicationDto>>(apps);
            return OperationResult<List<VolunteerApplicationDto>>.Success(dtos);
        }

        public async Task<OperationResult> ApproveAsync(Guid coordinatorId, Guid applicationId, string? note)
        {
            var application = await _applications.GetByIdAsync(applicationId);
            if (application is null)
                return OperationResult.Failure("Application not found.");

            if (application.Venue.CoordinatorId != coordinatorId)
                return OperationResult.Failure("Forbidden: you can only manage applications for your own venues.");

            if (application.Status != VolunteerApplicationStatus.Pending)
                return OperationResult.Failure("Only pending applications can be approved.");

            // Rule: only one approved venue per volunteer
            if (await _applications.HasApplicationWithStatusAsync(application.VolunteerId, VolunteerApplicationStatus.Approved))
                return OperationResult.Failure("This volunteer is already approved at another venue.");

            application.Status = VolunteerApplicationStatus.Approved;
            application.ReviewedByCoordinatorId = coordinatorId;
            application.ReviewedAt = DateTime.UtcNow;
            application.DecisionNote = note;

            await _applications.UpdateAsync(application);

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeclineAsync(Guid coordinatorId, Guid applicationId, string? note)
        {
            var application = await _applications.GetByIdAsync(applicationId);
            if (application is null)
                return OperationResult.Failure("Application not found.");

            if (application.Venue.CoordinatorId != coordinatorId)
                return OperationResult.Failure("Forbidden: you can only manage applications for your own venues.");

            if (application.Status != VolunteerApplicationStatus.Pending)
                return OperationResult.Failure("Only pending applications can be declined.");

            application.Status = VolunteerApplicationStatus.Declined;
            application.ReviewedByCoordinatorId = coordinatorId;
            application.ReviewedAt = DateTime.UtcNow;
            application.DecisionNote = note;

            await _applications.UpdateAsync(application);

            return OperationResult.Success();
        }

        public async Task<OperationResult<List<VolunteerProfileDto>>> GetApprovedVolunteersForVenueAsync(Guid venueId)
        {
            var volunteerIds = await _applications.GetApprovedVolunteerIdsForVenueAsync(venueId);

            if (volunteerIds.Count == 0)
                return OperationResult<List<VolunteerProfileDto>>.Success(new List<VolunteerProfileDto>());

            var profiles = new List<VolunteerProfileDto>();

            foreach (var id in volunteerIds)
            {
                var profile = await _profiles.GetByVolunteerIdAsync(id);
                if (profile is null) continue;

                profiles.Add(_mapper.Map<VolunteerProfileDto>(profile));
            }

            // Load join rows (MUST include Subject in repo)
            var rawSubjects = await _subjects.GetVolunteerSubjectsAsync(volunteerIds);

            foreach (var p in profiles)
            {
                p.Subjects = rawSubjects
                    .Where(s => s.VolunteerId == p.VolunteerId)
                    .Select(s => new VolunteerSubjectDto
                    {
                        Subject = new Application.Subjects.Dtos.SubjectDto
                        {
                            Id = s.Subject.Id,
                            Name = s.Subject.Name,
                            Icon = s.Subject.Icon
                        },
                        ConfidenceLevel = s.ConfidenceLevel
                    })
                    .ToList();
            }

            return OperationResult<List<VolunteerProfileDto>>.Success(profiles);
        }
    }
}
