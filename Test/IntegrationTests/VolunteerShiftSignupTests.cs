using Application.Auth.Dtos;
using Domain.Models.Entities.Venues;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Test.IntegrationTests
{
    [TestFixture]
    public class VolunteerShiftSignupTests
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
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Post_Volunteers_Shifts_Creates_Shift()
        {
            // Arrange
            var (token, volunteerId) = await RegisterAndLoginVolunteerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var timeSlotId = await SeedVenueTimeslotAndApprovalAsync(volunteerId);

            // Act
            var response = await _client.PostAsJsonAsync("/api/volunteers/shifts", new
            {
                timeSlotId,
                notes = "I can help!"
            });

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.GetProperty("isSuccess").GetBoolean(), Is.True);
            Assert.That(doc.RootElement.GetProperty("data").GetProperty("timeSlotId").GetGuid(), Is.EqualTo(timeSlotId));
        }

        [Test]
        public async Task Post_Volunteers_Shifts_Cannot_SignUp_Twice_For_Same_Shift()
        {
            // Arrange
            var (token, volunteerId) = await RegisterAndLoginVolunteerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var timeSlotId = await SeedVenueTimeslotAndApprovalAsync(volunteerId);

            // Act 1 - first signup OK
            var first = await _client.PostAsJsonAsync("/api/volunteers/shifts", new { timeSlotId });
            Assert.That(first.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Act 2 - second signup should fail
            var second = await _client.PostAsJsonAsync("/api/volunteers/shifts", new { timeSlotId });

            // Assert
            Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var body = await second.Content.ReadAsStringAsync();
            Assert.That(body, Does.Contain("\"isSuccess\":false"));
            Assert.That(body.ToLowerInvariant(), Does.Contain("already"));
        }

        // ----------------------------
        // Helpers
        // ----------------------------

        private async Task<(string token, Guid userId)> RegisterAndLoginVolunteerAsync()
        {
            // Register
            var email = $"vol.{Guid.NewGuid():N}@test.com";

            var registerDto = new RegisterUserDto
            {
                FirstName = "Test",
                LastName = "Volunteer",
                Email = email,
                Password = "Password123!",
                Role = UserRole.Volunteer
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
            Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Login
            var loginDto = new LoginUserDto
            {
                Email = email,
                Password = "Password123!"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var json = await loginResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var data = doc.RootElement.GetProperty("data");

            var token =
                TryGetString(data, "accessToken") ??
                TryGetString(data, "token");

            Assert.That(token, Is.Not.Null.And.Not.Empty, "Could not find access token in login response.");

            var userIdString =
                TryGetString(data, "userId") ??
                TryGetString(data, "id");

            Assert.That(userIdString, Is.Not.Null.And.Not.Empty, "Could not find userId in login response.");

            return (token!, Guid.Parse(userIdString!));
        }

        private async Task<Guid> SeedVenueTimeslotAndApprovalAsync(Guid volunteerId)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var venue = new Venue
            {
                Id = Guid.NewGuid(),
                Name = "Test Venue",
                Address = "Test Address",
                City = "Gothenburg",
                IsActive = true
            };

            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                DayOfWeek = WeekDay.Monday,
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                MaxStudents = 10,
                IsRecurring = true,
                SpecificDate = null,
                Status = TimeSlotStatus.Open // adjust if your enum differs
            };

            var application = new VolunteerApplication
            {
                Id = Guid.NewGuid(),
                VolunteerId = volunteerId,
                VenueId = venue.Id,
                Status = VolunteerApplicationStatus.Approved,
                AppliedAt = DateTime.UtcNow
            };

            db.Venues.Add(venue);
            db.TimeSlots.Add(timeSlot);
            db.VolunteerApplications.Add(application);

            await db.SaveChangesAsync();
            return timeSlot.Id;
        }

        private static string? TryGetString(JsonElement obj, string prop)
        {
            return obj.TryGetProperty(prop, out var el) && el.ValueKind == JsonValueKind.String
                ? el.GetString()
                : null;
        }
    }
}
