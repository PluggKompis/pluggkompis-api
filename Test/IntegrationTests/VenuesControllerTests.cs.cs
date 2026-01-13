using Application.Auth.Dtos;
using Application.Venues.Dtos;
using Domain.Models.Common;
using Domain.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Test.IntegrationTests;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class VenuesControllerTests
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
        public void Factory_Should_Have_JwtSettings_Configured()
        {
            // Arrange & Act
            using var scope = _factory.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Assert
            var secret = config["JwtSettings:Secret"];
            var issuer = config["JwtSettings:Issuer"];
            var audience = config["JwtSettings:Audience"];

            Console.WriteLine($"Secret: {secret}");
            Console.WriteLine($"Issuer: {issuer}");
            Console.WriteLine($"Audience: {audience}");

            Assert.That(secret, Is.Not.Null.And.Not.Empty, "JWT Secret should be configured");
            Assert.That(issuer, Is.EqualTo("PluggKompis.Test"), "JWT Issuer should match test config");
        }


        [Test]
        public async Task GetVenues_WithoutAuth_ReturnsOk()
        {
            // Arrange
            ClearAuthToken();

            // Act
            var response = await _client.GetAsync("/api/venues");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetVenues_WithNoVenues_ReturnsEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/api/venues");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var result = await response.Content
                .ReadFromJsonAsync<OperationResult<PagedResult<VenueDto>>>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Items, Is.Empty);
            Assert.That(result.Data.TotalCount, Is.EqualTo(0));
        }


        [Test]
        public async Task CreateVenue_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            ClearAuthToken();  // Make sure no token is set

            var request = new CreateVenueRequest
            {
                Name = "Test Bibliotek",
                Address = "Testgatan 1",
                City = "Teststad",
                PostalCode = "123 45",
                Description = "Test description",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/venues", request);

            // Assert - Print response for debugging
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task CreateVenue_AsCoordinator_ReturnsCreated()
        {
            // Arrange
            SetAuthToken(_coordinatorToken!);  // Use the token from SetUp

            var request = new CreateVenueRequest
            {
                Name = "Test Bibliotek",
                Address = "Testgatan 1",
                City = "Teststad",
                PostalCode = "123 45",
                Description = "Test description for integration test",
                ContactEmail = "contact@test.se",
                ContactPhone = "070-123 45 67"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/venues", request);

            // Assert - Print detailed error if it fails
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
                Console.WriteLine($"Token being used: {_coordinatorToken?.Substring(0, 20)}...");
            }

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                $"Expected Created but got {response.StatusCode}");

            var result = await response.Content
                .ReadFromJsonAsync<OperationResult<VenueDto>>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Name, Is.EqualTo(request.Name));
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
