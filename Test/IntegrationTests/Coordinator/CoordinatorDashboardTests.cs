using Application.Auth.Dtos;
using Application.Coordinator.Dtos;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Venues;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Test.IntegrationTests.Extensions;

namespace Test.IntegrationTests.Coordinator
{
    [TestFixture]
    public class CoordinatorDashboardTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;
        private string _coordinatorToken = null!;
        private Guid _coordinatorId;

        [SetUp]
        public async Task SetUp()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();

            (_coordinatorToken, _coordinatorId) = await RegisterCoordinatorAsync();
            SeedVenueAndData(_coordinatorId);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Get_Coordinator_Dashboard_Returns_Data()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _coordinatorToken);

            // Act
            var response = await _client.GetAsync("/api/coordinator/dashboard");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var result =
                await response.Content.ReadFromJsonWithEnumAsync<OperationResult<CoordinatorDashboardDto>>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            Assert.That(result.Data!.TotalVolunteers, Is.GreaterThan(0));
            Assert.That(result.Data.SubjectCoverage, Is.Not.Empty);
        }

        private async Task<(string token, Guid userId)> RegisterCoordinatorAsync()
        {
            var email = $"coord.{Guid.NewGuid()}@test.com";

            var dto = new RegisterUserDto
            {
                FirstName = "Test",
                LastName = "Coordinator",
                Email = email,
                Password = "Password123!",
                Role = UserRole.Coordinator
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
            var result = await response.Content.ReadFromJsonWithEnumAsync<OperationResult<AuthResponseDto>>();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db.Users.First(u => u.Email == email);

            return (result!.Data!.Token, user.Id);
        }

        private void SeedVenueAndData(Guid coordinatorId)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var venue = new Venue
            {
                Id = Guid.NewGuid(),
                Name = "Dashboard Venue",
                Address = "Testgatan 1",
                City = "Göteborg",
                PostalCode = "12345",
                Description = "Test venue",
                ContactEmail = "venue@test.se",
                ContactPhone = "0701234567",
                CoordinatorId = coordinatorId,
                IsActive = true
            };

            db.Venues.Add(venue);

            // Create a subject (required for SubjectCoverage join)
            var subject = new Domain.Models.Entities.Subjects.Subject
            {
                Id = Guid.NewGuid(),
                Name = "Matematik",
                Icon = "➗"
            };
            db.Subjects.Add(subject);


            // Create an active timeslot THIS WEEK so SubjectCoverage has "demand" data
            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                DayOfWeek = (WeekDay)DateTime.UtcNow.DayOfWeek, // or pick a specific WeekDay
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                MaxStudents = 10,
                Status = TimeSlotStatus.Open,

                // IMPORTANT: make it active this week
                IsRecurring = true,
                RecurringStartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1)),
                RecurringEndDate = null
            };

            db.TimeSlots.Add(timeSlot);

            // Link subject to timeslot (required for SubjectCoverage to include "Matematik")
            db.Set<TimeSlotSubject>().Add(new TimeSlotSubject
            {
                TimeSlotId = timeSlot.Id,
                SubjectId = subject.Id
            });

            // Approved volunteer linked to venue
            var volunteerId = Guid.NewGuid();

            db.VolunteerApplications.Add(new VolunteerApplication
            {
                Id = Guid.NewGuid(),
                VolunteerId = volunteerId,
                VenueId = venue.Id,
                Status = VolunteerApplicationStatus.Approved,
                AppliedAt = DateTime.UtcNow
            });

            // VolunteerSubject join row (required for SubjectCoverage join)
            db.Set<Domain.Models.Entities.JoinEntities.VolunteerSubject>().Add(new Domain.Models.Entities.JoinEntities.VolunteerSubject
            {
                VolunteerId = volunteerId,
                SubjectId = subject.Id,
                ConfidenceLevel = ConfidenceLevel.Intermediate
            });

            db.SaveChanges();
        }

        [Test]
        public async Task Get_Dashboard_As_NonCoordinator_Returns_Forbidden()
        {
            // Arrange
            var (token, _) = await RegisterVolunteerAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/coordinator/dashboard");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        private async Task<(string token, Guid userId)> RegisterVolunteerAsync()
        {
            var email = $"vol.{Guid.NewGuid()}@test.com";

            var dto = new RegisterUserDto
            {
                FirstName = "Test",
                LastName = "Volunteer",
                Email = email,
                Password = "Password123!",
                Role = UserRole.Volunteer
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
            var result = await response.Content.ReadFromJsonWithEnumAsync<OperationResult<AuthResponseDto>>();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db.Users.First(u => u.Email == email);

            return (result!.Data!.Token, user.Id);
        }
    }
}
