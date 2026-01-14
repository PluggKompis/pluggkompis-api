using Application.Auth.Dtos;
using Domain.Models.Common;
using Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Test.IntegrationTests
{
    [TestFixture]
    public class ChildrenControllerTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;
        private string? _parentToken1;
        private string? _parentToken2;

        [SetUp]
        public async Task SetUp()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();

            _parentToken1 = await GetParentTokenAsync("parent.one");
            _parentToken2 = await GetParentTokenAsync("parent.two");
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        /// <summary>
        /// Registers a parent and returns their JWT token
        /// </summary>
        private async Task<string> GetParentTokenAsync(string prefix)
        {
            var registerDto = new RegisterUserDto
            {
                FirstName = "Test",
                LastName = "Parent",
                Email = $"{prefix}.{Guid.NewGuid()}@test.com",
                Password = "Password123!",
                Role = UserRole.Parent
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to register parent: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();

            return result?.Data?.Token ?? throw new Exception("No token received");
        }

        [Test]
        public async Task Post_Children_CreatesChild()
        {
            // Arrange
            SetAuthToken(_parentToken1!);

            // Use your CreateChildRequest if you have it; otherwise anonymous object works.
            var request = new
            {
                firstName = "Ali",
                birthYear = 2016,
                schoolGrade = "3"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/children", request);

            // Debug
            if (response.StatusCode != HttpStatusCode.Created)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // We only need to confirm envelope success + some fields exist.
            var result = await response.Content.ReadFromJsonAsync<OperationResult<ChildDtoTest>>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Data.FirstName, Is.EqualTo("Ali"));
        }

        [Test]
        public async Task Get_Children_ReturnsOnlyParentsChildren()
        {
            // Arrange: Parent1 creates one child
            SetAuthToken(_parentToken1!);
            var create1 = await _client.PostAsJsonAsync("/api/children", new
            {
                firstName = "ChildA",
                birthYear = 2015,
                schoolGrade = "4"
            });
            Assert.That(create1.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Arrange: Parent2 creates one child
            SetAuthToken(_parentToken2!);
            var create2 = await _client.PostAsJsonAsync("/api/children", new
            {
                firstName = "ChildB",
                birthYear = 2017,
                schoolGrade = "2"
            });
            Assert.That(create2.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Act: Parent1 fetches children
            SetAuthToken(_parentToken1!);
            var response = await _client.GetAsync("/api/children");

            // Debug
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var result = await response.Content.ReadFromJsonAsync<OperationResult<List<ChildDtoTest>>>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            // Parent1 should only see ChildA
            Assert.That(result.Data!.Count, Is.EqualTo(1));
            Assert.That(result.Data[0].FirstName, Is.EqualTo("ChildA"));
        }

        [Test]
        public async Task Put_Children_EditingOtherParentsChild_Returns403()
        {
            // Arrange: Parent1 creates a child
            SetAuthToken(_parentToken1!);

            var createResponse = await _client.PostAsJsonAsync("/api/children", new
            {
                firstName = "OwnedChild",
                birthYear = 2014,
                schoolGrade = "5"
            });

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var created = await createResponse.Content.ReadFromJsonAsync<OperationResult<ChildDtoTest>>();
            Assert.That(created, Is.Not.Null);
            Assert.That(created!.IsSuccess, Is.True);
            Assert.That(created.Data, Is.Not.Null);

            var childId = created.Data!.Id;

            // Act: Parent2 tries to update Parent1's child
            SetAuthToken(_parentToken2!);

            var updateRequest = new
            {
                firstName = "HackedName",
                birthYear = 2014,
                schoolGrade = "5"
            };

            var updateResponse = await _client.PutAsJsonAsync($"/api/children/{childId}", updateRequest);

            // Debug
            if (updateResponse.StatusCode != HttpStatusCode.Forbidden)
            {
                var content = await updateResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {updateResponse.StatusCode}");
                Console.WriteLine($"Content: {content}");
            }

            // Assert
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
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

        // Minimal DTO for deserializing the response envelope in tests
        private class ChildDtoTest
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; } = default!;
            public int BirthYear { get; set; }
            public string SchoolGrade { get; set; } = default!;
        }
    }
}
