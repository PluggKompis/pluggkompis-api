//using Application.Auth.Dtos;
//using Domain.Models.Common;
//using Domain.Models.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http.Json;
//using System.Text;
//using System.Threading.Tasks;

//namespace Test.IntegrationTests.Auth
//{
//    [TestFixture]
//    public class AuthEndpointsTests
//    {
//        private CustomWebApplicationFactory _factory = default!;
//        private HttpClient _client = default!;

//        [SetUp]
//        public void SetUp()
//        {
//            _factory = new CustomWebApplicationFactory();
//            _client = _factory.CreateClient();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _client.Dispose();
//            _factory.Dispose();
//        }

//        [Test]
//        public async Task Post_Register_Should_Return201_When_Valid()
//        {
//            var dto = new RegisterUserDto
//            {
//                FirstName = "Mohanned",
//                LastName = "Test",
//                Email = "mohanned.integration@test.com",
//                Password = "Password123!",
//                Role = UserRole.Student
//            };

//            var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

//            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

//            var result = await response.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();
//            Assert.That(result, Is.Not.Null);
//            Assert.That(result!.IsSuccess, Is.True);
//            Assert.That(result.Data, Is.Not.Null);
//            Assert.That(result.Data!.Token, Is.Not.Empty);
//        }

//        [Test]
//        public async Task Post_Login_Should_Return200_When_CredentialsAreValid()
//        {
//            // Arrange: Register first
//            var register = new RegisterUserDto
//            {
//                FirstName = "Mohanned",
//                LastName = "Test",
//                Email = "login.integration@test.com",
//                Password = "Password123!",
//                Role = UserRole.Student
//            };

//            var regResponse = await _client.PostAsJsonAsync("/api/auth/register", register);
//            Assert.That(regResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

//            // Act: Login
//            var login = new LoginUserDto
//            {
//                Email = register.Email,
//                Password = register.Password
//            };

//            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", login);

//            // Assert
//            Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

//            var result = await loginResponse.Content.ReadFromJsonAsync<OperationResult<AuthResponseDto>>();
//            Assert.That(result, Is.Not.Null);
//            Assert.That(result!.IsSuccess, Is.True);
//            Assert.That(result.Data, Is.Not.Null);
//            Assert.That(result.Data!.Token, Is.Not.Empty);
//        }
//    }
//}
