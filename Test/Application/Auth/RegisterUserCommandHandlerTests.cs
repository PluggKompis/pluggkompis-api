using System.Linq.Expressions;
using Application.Auth.Commands.Register;
using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Entities.Users;
using Domain.Models.Enums;
using FakeItEasy;

namespace Test.Application.Auth
{
    [TestFixture]
    public class RegisterUserCommandHandlerTests
    {
        private IAuthRepository _authRepository = default!;
        private IPasswordHasher _passwordHasher = default!;
        private ITokenService _tokenService = default!;
        private IRefreshTokenGenerator _refreshTokenGenerator = default!;
        private IMapper _mapper = default!;

        private RegisterUserCommandHandler _sut = default!;

        [SetUp]
        public void SetUp()
        {
            _authRepository = A.Fake<IAuthRepository>();
            _passwordHasher = A.Fake<IPasswordHasher>();
            _tokenService = A.Fake<ITokenService>();
            _refreshTokenGenerator = A.Fake<IRefreshTokenGenerator>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterUserDto, User>()
                    .ForMember(d => d.PasswordHash, opt => opt.Ignore());

                cfg.CreateMap<User, UserDtoResponse>()
                    .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));
            });

            _mapper = config.CreateMapper();

            _sut = new RegisterUserCommandHandler(
                _authRepository,
                _passwordHasher,
                _tokenService,
                _refreshTokenGenerator,
                _mapper);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_When_ValidData()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                FirstName = "Mohanned",
                LastName = "Test",
                Email = "mohanned@test.com",
                Password = "Password123!",
                Role = UserRole.Student
            };

            A.CallTo(() => _authRepository.EmailExistsAsync(dto.Email, A<CancellationToken>._))
                .Returns(false);

            A.CallTo(() => _passwordHasher.Hash(dto.Password))
                .Returns("hashed");

            var refreshExpiresAt = DateTime.UtcNow.AddDays(7);

            A.CallTo(() => _refreshTokenGenerator.Generate())
                .Returns(("refresh-token", refreshExpiresAt));

            A.CallTo(() => _tokenService.GenerateJwt(A<User>._))
                .Returns("jwt-token");

            // Important: handler now calls SaveChangesAsync
            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .Returns(1);

            // Act
            var result = await _sut.Handle(new RegisterUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Token, Is.EqualTo("jwt-token"));
            Assert.That(result.Data!.RefreshToken, Is.EqualTo("refresh-token"));
            Assert.That(result.Data!.User, Is.Not.Null);
            Assert.That(result.Data!.User!.Email, Is.EqualTo(dto.Email));

            A.CallTo(() => _authRepository.AddAsync(A<User>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_EmailAlreadyExists()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                FirstName = "A",
                LastName = "B",
                Email = "duplicate@test.com",
                Password = "Password123!",
                Role = UserRole.Student
            };

            A.CallTo(() => _authRepository.EmailExistsAsync(dto.Email, A<CancellationToken>._))
                .Returns(true);

            // Act
            var result = await _sut.Handle(new RegisterUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Does.Contain("already").IgnoreCase);

            A.CallTo(() => _authRepository.AddAsync(A<User>._, A<CancellationToken>._))
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
            var dto = new RegisterUserDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@savefail.com",
                Password = "Password123!",
                Role = UserRole.Student
            };

            A.CallTo(() => _authRepository.EmailExistsAsync(dto.Email, A<CancellationToken>._))
                .Returns(false);

            A.CallTo(() => _passwordHasher.Hash(dto.Password))
                .Returns("hashed");

            A.CallTo(() => _refreshTokenGenerator.Generate())
                .Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

            // Simulate "nothing saved"
            A.CallTo(() => _authRepository.SaveChangesAsync(A<CancellationToken>._))
                .Returns(0);

            // Act
            var result = await _sut.Handle(new RegisterUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Does.Contain("persist").IgnoreCase);

            A.CallTo(() => _tokenService.GenerateJwt(A<User>._))
                .MustNotHaveHappened();
        }
    }
}
