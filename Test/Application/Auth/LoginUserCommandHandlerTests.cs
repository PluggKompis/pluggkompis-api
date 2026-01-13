using Application.Auth.Commands.Login;
using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Entities.Users;
using Domain.Models.Enums;
using FakeItEasy;
using System.Linq.Expressions;

namespace Test.Application.Auth
{
    [TestFixture]
    public class LoginUserCommandHandlerTests
    {
        private IGenericRepository<User> _users = default!;
        private IPasswordHasher _passwordHasher = default!;
        private ITokenService _tokenService = default!;
        private IRefreshTokenGenerator _refreshTokenGenerator = default!;
        private IMapper _mapper = default!;

        private LoginUserCommandHandler _sut = default!;

        [SetUp]
        public void SetUp()
        {
            _users = A.Fake<IGenericRepository<User>>();
            _passwordHasher = A.Fake<IPasswordHasher>();
            _tokenService = A.Fake<ITokenService>();
            _refreshTokenGenerator = A.Fake<IRefreshTokenGenerator>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDtoResponse>()
                    .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));
            });
            _mapper = config.CreateMapper();

            _sut = new LoginUserCommandHandler(
                _users,
                _passwordHasher,
                _tokenService,
                _refreshTokenGenerator,
                _mapper);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_When_ValidCredentials()
        {
            // Arrange
            var dto = new LoginUserDto { Email = "user@test.com", Password = "Password123!" };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = "hashed",
                Role = UserRole.Student,
                IsActive = true,
                FirstName = "A",
                LastName = "B"
            };

            A.CallTo(() => _users.FindAsync(A<Expression<Func<User, bool>>>._))
                .Returns(new[] { user });

            A.CallTo(() => _passwordHasher.Verify(dto.Password, user.PasswordHash))
                .Returns(true);

            A.CallTo(() => _refreshTokenGenerator.Generate())
                .Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

            A.CallTo(() => _tokenService.GenerateJwt(user))
                .Returns("jwt-token");

            // Act
            var result = await _sut.Handle(new LoginUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Token, Is.EqualTo("jwt-token"));
            Assert.That(result.Data!.RefreshToken, Is.EqualTo("refresh-token"));

            A.CallTo(() => _users.UpdateAsync(user)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_InvalidCredentials()
        {
            // Arrange
            var dto = new LoginUserDto { Email = "user@test.com", Password = "WrongPassword" };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = "hashed",
                Role = UserRole.Student,
                IsActive = true
            };

            A.CallTo(() => _users.FindAsync(A<Expression<Func<User, bool>>>._))
                .Returns(new[] { user });

            A.CallTo(() => _passwordHasher.Verify(dto.Password, user.PasswordHash))
                .Returns(false);

            // Act
            var result = await _sut.Handle(new LoginUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Does.Contain("Invalid credentials").IgnoreCase);

            A.CallTo(() => _users.UpdateAsync(A<User>._)).MustNotHaveHappened();
            A.CallTo(() => _tokenService.GenerateJwt(A<User>._)).MustNotHaveHappened();
        }
    }
}
