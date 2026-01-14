using Application.Auth.Dtos;
using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using Domain.Models.Enums;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Test.IntegrationTests.Auth
{
    [TestFixture]
    public class AuthEndpointsTests
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
        public async Task Post_Register_Should_Return201_When_Valid()
        {
            var dto = new RegisterUserDto
            {
                FirstName = "Mohanned",
                LastName = "Test",
                Email = "mohanned. integration@test.com",
                Password = "Password123!",
                Role = UserRole.Student
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var result = await response.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Token, Is.Not.Empty);
        }
    }
}
