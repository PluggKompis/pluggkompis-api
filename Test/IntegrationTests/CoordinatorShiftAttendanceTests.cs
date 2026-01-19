using Application.Auth.Dtos;
using Domain.Models.Common;
using Domain.Models.Entities.Venues;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.VolunteerShifts.Dtos;

namespace Test.IntegrationTests
{
    [TestFixture]
    public class CoordinatorShiftAttendanceTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;

        [SetUp]
        public void SetUp()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Test]
        public async Task Put_Coordinator_Shifts_Attendance_Coordinator_Marks_Attendance()
        {
            // Arrange: seed coordinator + volunteer + venue + timeslot + approved application + shift
            var setup = await SeedScenarioAsync();

            SetAuthToken(setup.CoordinatorToken);

            var request = new
            {
                isAttended = true,
                notes = "Arrived on time and did great."
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                $"/api/coordinator/shifts/{setup.VolunteerShiftId}/attendance",
                request);

            // Debug
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Verify response envelope
            var result = await response.Content.ReadFromJsonAsync<OperationResult<VolunteerShiftDto>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            Assert.That(result.Data!.Id, Is.EqualTo(setup.VolunteerShiftId));
            Assert.That(result.Data.IsAttended, Is.True);
            Assert.That(result.Data.Notes, Is.EqualTo("Arrived on time and did great."));
            Assert.That(result.Data.Status, Is.EqualTo(VolunteerShiftStatus.Completed));

            // Verify persisted in DB
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var saved = await db.VolunteerShifts.FindAsync(setup.VolunteerShiftId);
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved!.IsAttended, Is.True);
            Assert.That(saved.Notes, Is.EqualTo("Arrived on time and did great."));
            Assert.That(saved.Status, Is.EqualTo(VolunteerShiftStatus.Completed));
        }

        [Test]
        public async Task Put_Coordinator_Shifts_Attendance_NonCoordinator_Returns_Forbidden()
        {
            // Arrange
            var setup = await SeedScenarioAsync();

            SetAuthToken(setup.VolunteerToken);

            var request = new
            {
                isAttended = true,
                notes = "Trying to cheat"
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                $"/api/coordinator/shifts/{setup.VolunteerShiftId}/attendance",
                request);

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Content: {content}");

            // Assert
            Assert.That(
                response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized,
                Is.True,
                $"Expected 403 Forbidden (or 401 Unauthorized) but got {(int)response.StatusCode} {response.StatusCode}. Body: {content}"
            );
        }

        // ----------------------------
        // Scenario seeding + auth helpers
        // ----------------------------

        private async Task<(string CoordinatorToken, Guid CoordinatorId, string VolunteerToken, Guid VolunteerId, Guid VenueId, Guid TimeSlotId, Guid VolunteerShiftId)>
            SeedScenarioAsync()
        {
            // Register coordinator + volunteer (token from register)
            var (coordinatorToken, coordinatorId) = await RegisterUserAndGetTokenAndIdAsync(UserRole.Coordinator);
            var (volunteerToken, volunteerId) = await RegisterUserAndGetTokenAndIdAsync(UserRole.Volunteer);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Venue coordinated by this coordinator
            var venue = new Venue
            {
                Id = Guid.NewGuid(),
                Name = "Test Venue",
                Address = "Testgatan 1",
                City = "Göteborg",
                PostalCode = "123 45",
                Description = "Venue for attendance test",
                ContactEmail = "venue@test.se",
                ContactPhone = "070-000 00 00",
                CoordinatorId = coordinatorId,
                IsActive = true
            };

            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18),
                MaxStudents = 20,
                IsRecurring = true,
                SpecificDate = null,
                Status = TimeSlotStatus.Open
            };

            // Volunteer approved for venue
            var approval = new VolunteerApplication
            {
                Id = Guid.NewGuid(),
                VolunteerId = volunteerId,
                VenueId = venue.Id,
                Status = VolunteerApplicationStatus.Approved,
                AppliedAt = DateTime.UtcNow
            };

            // Shift (as if volunteer already signed up)
            var shift = new VolunteerShift
            {
                Id = Guid.NewGuid(),
                TimeSlotId = timeSlot.Id,
                VolunteerId = volunteerId,
                Status = VolunteerShiftStatus.Confirmed,
                IsAttended = false,
                Notes = null
            };

            db.Venues.Add(venue);
            db.TimeSlots.Add(timeSlot);
            db.VolunteerApplications.Add(approval);
            db.VolunteerShifts.Add(shift);

            await db.SaveChangesAsync();

            return (coordinatorToken, coordinatorId, volunteerToken, volunteerId, venue.Id, timeSlot.Id, shift.Id);
        }

        private async Task<(string token, Guid userId)> RegisterUserAndGetTokenAndIdAsync(UserRole role)
        {
            var emailPrefix = role == UserRole.Coordinator ? "coordinator" : "volunteer";
            var email = $"{emailPrefix}.{Guid.NewGuid()}@test.com";

            var registerDto = new RegisterUserDto
            {
                FirstName = "Test",
                LastName = role.ToString(),
                Email = email,
                Password = "Password123!",
                Role = role
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to register {role}: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();
            var token = result?.Data?.Token ?? throw new Exception($"No token received when registering {role}");

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            Assert.That(user, Is.Not.Null, $"Could not find registered {role} user in DB.");

            return (token, user!.Id);
        }

        private void SetAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
