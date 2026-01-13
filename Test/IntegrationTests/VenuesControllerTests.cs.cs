using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class VenuesControllerTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task GetVenues_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/venues");

            Assert.That(response.IsSuccessStatusCode, Is.True);

            var result = await response.Content
                .ReadFromJsonAsync<OperationResult<PagedResult<VenueDto>>>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
        }

        [Test]
        public async Task CreateVenue_ReturnsCreated()
        {
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

            var response = await _client.PostAsJsonAsync("/api/venues", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var result = await response.Content
                .ReadFromJsonAsync<OperationResult<VenueDto>>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Name, Is.EqualTo(request.Name));
            Assert.That(result.Data.City, Is.EqualTo(request.City));
        }

        [Test]
        public async Task GetVenueById_WithValidId_ReturnsVenue()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/venues", new CreateVenueRequest
            {
                Name = "Test Bibliotek",
                Address = "Testgatan 1",
                City = "Teststad",
                PostalCode = "123 45",
                Description = "Test description",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67"
            });

            var createResult = await createResponse.Content
                .ReadFromJsonAsync<OperationResult<VenueDto>>();

            var venueId = createResult!.Data!.Id;

            var response = await _client.GetAsync($"/api/venues/{venueId}");

            Assert.That(response.IsSuccessStatusCode, Is.True);

            var result = await response.Content
                .ReadFromJsonAsync<OperationResult<VenueDetailDto>>();

            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data!.Id, Is.EqualTo(venueId));
        }

        [Test]
        public async Task GetVenueById_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync($"/api/venues/{Guid.NewGuid()}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task UpdateVenue_WithValidData_ReturnsSuccess()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/venues", new CreateVenueRequest
            {
                Name = "Original Name",
                Address = "Testgatan 1",
                City = "Teststad",
                PostalCode = "123 45",
                Description = "Original description",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67"
            });

            var createResult = await createResponse.Content
                .ReadFromJsonAsync<OperationResult<VenueDto>>();

            var venueId = createResult!.Data!.Id;

            var updateRequest = new UpdateVenueRequest
            {
                Name = "Updated Name",
                Address = "Testgatan 1",
                City = "Teststad",
                PostalCode = "123 45",
                Description = "Updated description",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67",
                IsActive = true
            };

            var response = await _client.PutAsJsonAsync($"/api/venues/{venueId}", updateRequest);

            Assert.That(response.IsSuccessStatusCode, Is.True);

            var result = await response.Content
                .ReadFromJsonAsync<OperationResult<VenueDto>>();

            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data!.Name, Is.EqualTo("Updated Name"));
            Assert.That(result.Data.Description, Is.EqualTo("Updated description"));
        }

        [Test]
        public async Task DeleteVenue_WithValidId_ReturnsNoContent()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/venues", new CreateVenueRequest
            {
                Name = "Test Bibliotek",
                Address = "Testgatan 1",
                City = "Teststad",
                PostalCode = "123 45",
                Description = "Test description",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67"
            });

            var createResult = await createResponse.Content
                .ReadFromJsonAsync<OperationResult<VenueDto>>();

            var venueId = createResult!.Data!.Id;

            var response = await _client.DeleteAsync($"/api/venues/{venueId}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var getResponse = await _client.GetAsync($"/api/venues/{venueId}");
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateVenue_TwiceForSameCoordinator_ReturnsError()
        {
            var request = new CreateVenueRequest
            {
                Name = "First Venue",
                Address = "Testgatan 1",
                City = "Teststad",
                PostalCode = "123 45",
                Description = "Test",
                ContactEmail = "test@test.se",
                ContactPhone = "070-123 45 67"
            };

            var response1 = await _client.PostAsJsonAsync("/api/venues", request);
            Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            request.Name = "Second Venue";
            var response2 = await _client.PostAsJsonAsync("/api/venues", request);

            Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
