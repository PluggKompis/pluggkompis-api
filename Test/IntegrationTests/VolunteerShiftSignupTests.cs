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
    public class VolunteerShiftSignupTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;
        private string? _volunteerToken;
        private Guid _volunteerId;

        [SetUp]
        public async Task SetUp()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();

            // Register volunteer and get token (same pattern as your TimeSlots tests)
            (_volunteerToken, _volunteerId) = await RegisterVolunteerAndGetTokenAndIdAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Test]
        public async Task Post_Volunteers_Shifts_Creates_Shift()
        {
            // Arrange
            SetAuthToken(_volunteerToken!);

            var timeSlotId = await SeedVenueTimeSlotAndApprovalAsync(_volunteerId);

            var request = new
            {
                timeSlotId,
                notes = "I can help!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/volunteers/shifts", request);

            // Debug
            if (response.StatusCode != HttpStatusCode.Created)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var result = await response.Content.ReadFromJsonAsync<OperationResult<VolunteerShiftDto>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.TimeSlotId, Is.EqualTo(timeSlotId));
        }

        [Test]
        public async Task Post_Volunteers_Shifts_Cannot_SignUp_Twice_For_Same_Shift()
        {
            // Arrange
            SetAuthToken(_volunteerToken!);

            var timeSlotId = await SeedVenueTimeSlotAndApprovalAsync(_volunteerId);

            // Act 1 - first signup OK
            var first = await _client.PostAsJsonAsync("/api/volunteers/shifts", new { timeSlotId });
            Assert.That(first.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Act 2 - second signup should fail
            var second = await _client.PostAsJsonAsync("/api/volunteers/shifts", new { timeSlotId });

            // Debug
            if (second.StatusCode != HttpStatusCode.BadRequest)
            {
                var content = await second.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {second.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var result = await second.Content.ReadFromJsonAsync<OperationResult<VolunteerShiftDto>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.False);
            Assert.That(result.Errors.Any(e => e.Contains("already", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        /// <summary>
        /// Register a volunteer and return (token, volunteerId). Token is returned from /register.
        /// volunteerId is loaded from DB using the unique email we registered with.
        /// </summary>
        private async Task<(string token, Guid volunteerId)> RegisterVolunteerAndGetTokenAndIdAsync()
        {
            var email = $"volunteer.{Guid.NewGuid()}@test.com";

            var registerDto = new RegisterUserDto
            {
                FirstName = "Test",
                LastName = "Volunteer",
                Email = email,
                Password = "Password123!",
                Role = UserRole.Volunteer
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to register volunteer: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();
            var token = result?.Data?.Token ?? throw new Exception("No token received from register");

            // Fetch userId from DB by email (same in-memory DB instance via factory services)
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            Assert.That(user, Is.Not.Null, "Could not find registered volunteer user in DB.");

            return (token, user!.Id);
        }

        private async Task<Guid> SeedVenueTimeSlotAndApprovalAsync(Guid volunteerId)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var venue = new Venue
            {
                Id = Guid.NewGuid(),
                Name = "Test Venue",
                Address = "Testgatan 1",
                City = "Göteborg",
                PostalCode = "123 45",
                Description = "Test venue for volunteer shifts",
                ContactEmail = "venue@test.se",
                ContactPhone = "070-123 45 67",
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

            // Approval required by business rule
            var approval = new VolunteerApplication
            {
                Id = Guid.NewGuid(),
                VolunteerId = volunteerId,
                VenueId = venue.Id,
                Status = VolunteerApplicationStatus.Approved,
                AppliedAt = DateTime.UtcNow
            };

            db.Venues.Add(venue);
            db.TimeSlots.Add(timeSlot);
            db.VolunteerApplications.Add(approval);

            await db.SaveChangesAsync();

            return timeSlot.Id;
        }

        private void SetAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
