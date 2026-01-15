using Application.Auth.Dtos;
using Application.TimeSlots.Dtos;
using Application.Venues.Dtos;
using Domain.Models.Common;
using Domain.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Test.IntegrationTests
{
    [TestFixture]
    public class TimeSlotsControllerTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;
        private string? _coordinatorToken;

        [SetUp]
        public async Task SetUp()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();

            // Get a coordinator token for authenticated tests
            _coordinatorToken = await GetCoordinatorTokenAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        /// <summary>
        /// Registers a coordinator and returns their JWT token
        /// </summary>
        private async Task<string> GetCoordinatorTokenAsync()
        {
            var registerDto = new RegisterUserDto
            {
                FirstName = "Test",
                LastName = "Coordinator",
                Email = $"coordinator.{Guid.NewGuid()}@test.com",
                Password = "Password123!",
                Role = UserRole.Coordinator
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to register coordinator: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();

            return result?.Data?.Token ?? throw new Exception("No token received");
        }

        [Test]
        public async Task CreateTimeSlot_WithValidData_ReturnsCreated()
        {
            // Arrange
            SetAuthToken(_coordinatorToken!);

            // Create a venue first
            var venueRequest = new CreateVenueRequest
            {
                Name = "Test Bibliotek",
                Address = "Testgatan 1",
                City = "Göteborg",
                PostalCode = "123 45",
                Description = "Test venue for timeslot",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67"
            };

            var venueResponse = await _client.PostAsJsonAsync("/api/venues", venueRequest);
            var venueResult = await venueResponse.Content.ReadFromJsonAsync<OperationResult<VenueDto>>();
            Assert.That(venueResult, Is.Not.Null);
            Assert.That(venueResult!.IsSuccess, Is.True);
            var venueId = venueResult.Data!.Id;

            // Seed a subject (or use a known subject ID from your DataSeeder)
            var subjectId = await SeedTestSubjectAsync();

            var timeSlotRequest = new CreateTimeSlotRequest
            {
                VenueId = venueId,
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(16),  // 16:00
                EndTime = TimeSpan.FromHours(18),    // 18:00
                MaxStudents = 20,
                IsRecurring = true,
                SpecificDate = null,
                SubjectIds = new List<Guid> { subjectId }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/timeslots", timeSlotRequest);

            // Debug
            if (response.StatusCode != HttpStatusCode.Created)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                $"Expected Created but got {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<OperationResult<TimeSlotDto>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.VenueId, Is.EqualTo(venueId));
            Assert.That(result.Data.DayOfWeek, Is.EqualTo(WeekDay.Monday));
            Assert.That(result.Data.StartTime, Is.EqualTo(TimeSpan.FromHours(16)));
            Assert.That(result.Data.EndTime, Is.EqualTo(TimeSpan.FromHours(18)));
            Assert.That(result.Data.MaxStudents, Is.EqualTo(20));
            Assert.That(result.Data.IsRecurring, Is.True);
            Assert.That(result.Data.Status, Is.EqualTo(TimeSlotStatus.Open));
            Assert.That(result.Data.AvailableSpots, Is.EqualTo(20)); // No bookings yet
        }

        [Test]
        public async Task CreateTimeSlot_WithOverlappingTime_ReturnsBadRequest()
        {
            // Arrange
            SetAuthToken(_coordinatorToken!);

            // Create venue
            var venueRequest = new CreateVenueRequest
            {
                Name = "Test Bibliotek",
                Address = "Testgatan 1",
                City = "Göteborg",
                PostalCode = "123 45",
                Description = "Test venue",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67"
            };

            var venueResponse = await _client.PostAsJsonAsync("/api/venues", venueRequest);
            var venueResult = await venueResponse.Content.ReadFromJsonAsync<OperationResult<VenueDto>>();
            Assert.That(venueResult, Is.Not.Null);
            var venueId = venueResult!.Data!.Id;

            var subjectId = await SeedTestSubjectAsync();

            // Create first timeslot
            var firstTimeSlot = new CreateTimeSlotRequest
            {
                VenueId = venueId,
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18),
                MaxStudents = 20,
                IsRecurring = true,
                SubjectIds = new List<Guid> { subjectId }
            };

            var firstResponse = await _client.PostAsJsonAsync("/api/timeslots", firstTimeSlot);
            Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Create overlapping timeslot
            var overlappingTimeSlot = new CreateTimeSlotRequest
            {
                VenueId = venueId,
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(17),  // Overlaps with 16-18
                EndTime = TimeSpan.FromHours(19),
                MaxStudents = 20,
                IsRecurring = true,
                SubjectIds = new List<Guid> { subjectId }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/timeslots", overlappingTimeSlot);

            // Debug
            if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var result = await response.Content.ReadFromJsonAsync<OperationResult<TimeSlotDto>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.False);
            Assert.That(result.Errors.Any(e => e.Contains("overlap", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        [Test]
        public async Task GetVenueTimeSlots_ReturnsSchedule()
        {
            // Arrange
            SetAuthToken(_coordinatorToken!);

            // Create venue
            var venueRequest = new CreateVenueRequest
            {
                Name = "Test Bibliotek",
                Address = "Testgatan 1",
                City = "Göteborg",
                PostalCode = "123 45",
                Description = "Test venue",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67"
            };

            var venueResponse = await _client.PostAsJsonAsync("/api/venues", venueRequest);
            var venueResult = await venueResponse.Content.ReadFromJsonAsync<OperationResult<VenueDto>>();
            var venueId = venueResult!.Data!.Id;

            var subjectId = await SeedTestSubjectAsync();

            // Create multiple timeslots
            var timeslot1 = new CreateTimeSlotRequest
            {
                VenueId = venueId,
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18),
                MaxStudents = 20,
                IsRecurring = true,
                SubjectIds = new List<Guid> { subjectId }
            };

            var timeslot2 = new CreateTimeSlotRequest
            {
                VenueId = venueId,
                DayOfWeek = WeekDay.Wednesday,
                StartTime = TimeSpan.FromHours(17),
                EndTime = TimeSpan.FromHours(19),
                MaxStudents = 15,
                IsRecurring = true,
                SubjectIds = new List<Guid> { subjectId }
            };

            await _client.PostAsJsonAsync("/api/timeslots", timeslot1);
            await _client.PostAsJsonAsync("/api/timeslots", timeslot2);

            // Act
            var response = await _client.GetAsync($"/api/venues/{venueId}/timeslots");

            // Debug
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var result = await response.Content.ReadFromJsonAsync<OperationResult<List<TimeSlotDto>>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Count, Is.EqualTo(2));

            // Verify ordering (Monday before Wednesday)
            Assert.That(result.Data[0].DayOfWeek, Is.EqualTo(WeekDay.Monday));
            Assert.That(result.Data[1].DayOfWeek, Is.EqualTo(WeekDay.Wednesday));
        }

        [Test]
        public async Task CreateTimeSlot_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            ClearAuthToken();

            var timeSlotRequest = new CreateTimeSlotRequest
            {
                VenueId = Guid.NewGuid(),
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18),
                MaxStudents = 20,
                IsRecurring = true,
                SubjectIds = new List<Guid> { Guid.NewGuid() }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/timeslots", timeSlotRequest);

            // Debug
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        /// <summary>
        /// Helper to seed a test subject in the database
        /// </summary>
        private async Task<Guid> SeedTestSubjectAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Infrastructure.Database.AppDbContext>();

            var subject = new Domain.Models.Entities.Subjects.Subject
            {
                Id = Guid.NewGuid(),
                Name = "Test Subject - Matematik",
                Icon = "📐"
            };

            db.Subjects.Add(subject);
            await db.SaveChangesAsync();

            return subject.Id;
        }

        private void SetAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private void ClearAuthToken()
        {
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
