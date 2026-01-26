using Application.Auth.Commands.Login;
using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Entities.Users;
using Domain.Models.Enums;
using FakeItEasy;

namespace Test.Application.Auth
{
    [TestFixture]
    public class LoginUserCommandHandlerTests
    {
        private IAuthRepository _authRepository = default!;
        private IPasswordHasher _passwordHasher = default!;
        private ITokenService _tokenService = default!;
        private IRefreshTokenGenerator _refreshTokenGenerator = default!;
        private IMapper _mapper = default!;

        private LoginUserCommandHandler _sut = default!;

        [SetUp]
        public void SetUp()
        {
            _authRepository = A.Fake<IAuthRepository>();
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
                _authRepository,
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

            A.CallTo(() => _authRepository.GetByEmailAsync(dto.Email, A<CancellationToken>._))
                .Returns(user);

            A.CallTo(() => _passwordHasher.Verify(dto.Password, user.PasswordHash))
                .Returns(true);

            A.CallTo(() => _refreshTokenGenerator.Generate())
                .Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

            A.CallTo(() => _tokenService.GenerateJwt(user))
                .Returns("jwt-token");

            // Important: handler now calls SaveChangesAsync
            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .Returns(1);

            // Act
            var result = await _sut.Handle(new LoginUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Token, Is.EqualTo("jwt-token"));
            Assert.That(result.Data!.RefreshToken, Is.EqualTo("refresh-token"));

            A.CallTo(() => _authRepository.UpdateAsync(user, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var dto = new LoginUserDto { Email = "missing@test.com", Password = "Password123!" };

            A.CallTo(() => _authRepository.GetByEmailAsync(dto.Email, A<CancellationToken>._))
                .Returns((User?)null);

            // Act
            var result = await _sut.Handle(new LoginUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Does.Contain("Invalid credentials").IgnoreCase);

            A.CallTo(() => _authRepository.UpdateAsync(A<User>._, A<CancellationToken>._))
                .MustNotHaveHappened();

            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .MustNotHaveHappened();

            A.CallTo(() => _tokenService.GenerateJwt(A<User>._))
                .MustNotHaveHappened();
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

            A.CallTo(() => _authRepository.GetByEmailAsync(dto.Email, A<CancellationToken>._))
                .Returns(user);

            A.CallTo(() => _passwordHasher.Verify(dto.Password, user.PasswordHash))
                .Returns(false);

            // Act
            var result = await _sut.Handle(new LoginUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Does.Contain("Invalid credentials").IgnoreCase);

            A.CallTo(() => _authRepository.UpdateAsync(A<User>._, A<CancellationToken>._))
                .MustNotHaveHappened();

            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .MustNotHaveHappened();

            A.CallTo(() => _tokenService.GenerateJwt(A<User>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UserIsInactive()
        {
            // Arrange
            var dto = new LoginUserDto { Email = "user@test.com", Password = "Password123!" };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = "hashed",
                Role = UserRole.Student,
                IsActive = false
            };

            A.CallTo(() => _authRepository.GetByEmailAsync(dto.Email, A<CancellationToken>._))
                .Returns(user);

            // Act
            var result = await _sut.Handle(new LoginUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Does.Contain("inactive").IgnoreCase);

            A.CallTo(() => _authRepository.UpdateAsync(A<User>._, A<CancellationToken>._))
                .MustNotHaveHappened();

            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .MustNotHaveHappened();

            A.CallTo(() => _tokenService.GenerateJwt(A<User>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_SaveChangesDoesNotPersist()
        {
            // Arrange
            var dto = new LoginUserDto { Email = "user@test.com", Password = "Password123!" };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = "hashed",
                Role = UserRole.Student,
                IsActive = true
            };

            A.CallTo(() => _authRepository.GetByEmailAsync(dto.Email, A<CancellationToken>._))
                .Returns(user);

            A.CallTo(() => _passwordHasher.Verify(dto.Password, user.PasswordHash))
                .Returns(true);

            A.CallTo(() => _refreshTokenGenerator.Generate())
                .Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

            // Simulate failure to persist
            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .Returns(0);

            // Act
            var result = await _sut.Handle(new LoginUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Does.Contain("persist").IgnoreCase);

            A.CallTo(() => _tokenService.GenerateJwt(A<User>._))
                .MustNotHaveHappened();
        }
    }
}
