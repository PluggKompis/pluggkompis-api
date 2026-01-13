using Application.Auth.Dtos;
using Domain.Models.Common;
using Domain.Models.Enums;
using System.Net;
using System.Net.Http.Json;

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

        //[Test]
        //public async Task Post_Login_Should_Return200_When_CredentialsAreValid()
        //{
        //    // Arrange: Register first
        //    var register = new RegisterUserDto
        //    {
        //        FirstName = "Mohanned",
        //        LastName = "Test",
        //        Email = "login.integration@test.com",
        //        Password = "Password123!",
        //        Role = UserRole.Student
        //    };

        //    var regResponse = await _client.PostAsJsonAsync("/api/auth/register", register);

        //    // Debug registration
        //    if (!regResponse.IsSuccessStatusCode)
        //    {
        //        var regError = await regResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine($"Registration failed: {regError}");
        //    }

        //    Assert.That(regResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created),
        //        $"Registration failed: {await regResponse.Content.ReadAsStringAsync()}");

        //    var regResult = await regResponse.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();
        //    Console.WriteLine($"Registration successful. User registered with email: {register.Email}");

        //    // Act: Login
        //    var login = new LoginUserDto
        //    {
        //        Email = register.Email,
        //        Password = register.Password
        //    };

        //    Console.WriteLine($"Attempting login with email: {login.Email}");

        //    var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", login);

        //    // Debug login
        //    var loginContent = await loginResponse.Content.ReadAsStringAsync();
        //    Console.WriteLine($"Login response status: {loginResponse.StatusCode}");
        //    Console.WriteLine($"Login response content: {loginContent}");

        //    if (!loginResponse.IsSuccessStatusCode)
        //    {
        //        // Try to parse as OperationResult to see error details
        //        var errorResult = await loginResponse.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();
        //        if (errorResult != null && errorResult.Errors.Any())
        //        {
        //            Console.WriteLine($"Login errors: {string.Join(", ", errorResult.Errors)}");
        //        }
        //    }

        //    // Assert
        //    Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        //    var result = await loginResponse.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result!.IsSuccess, Is.True);
        //    Assert.That(result.Data, Is.Not.Null);
        //    Assert.That(result.Data!.Token, Is.Not.Empty);
        //}
    }
}
