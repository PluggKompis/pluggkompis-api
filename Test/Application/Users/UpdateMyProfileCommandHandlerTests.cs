using Application.Common.Interfaces;
using Application.Users.Commands.UpdateMyProfile;
using Application.Users.Dtos;
using AutoMapper;
using Domain.Models.Entities.Users;
using FakeItEasy;

namespace Test.Application.Users
{
    [TestFixture]
    public class UpdateMyProfileCommandHandlerTests
    {
        private IGenericRepository<User> _users = default!;
        private IAuthRepository _authRepo = default!;
        private IMapper _mapper = default!;
        private UpdateMyProfileCommandHandler _sut = default!;

        [SetUp]
        public void SetUp()
        {
            _users = A.Fake<IGenericRepository<User>>();
            _authRepo = A.Fake<IAuthRepository>();
            _mapper = A.Fake<IMapper>();

            _sut = new UpdateMyProfileCommandHandler(_users, _authRepo, _mapper);
        }

        [Test]
        public async Task Handle_Should_UpdateOnlyFirstName_When_OnlyFirstNameProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FirstName = "OldFirst",
                LastName = "OldLast",
                Email = "old@test.com",
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);

            var cmd = new UpdateMyProfileCommand(userId, new UpdateMyProfileDto
            {
                FirstName = " NewFirst "
            });

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(user.FirstName, Is.EqualTo("NewFirst"));
            Assert.That(user.LastName, Is.EqualTo("OldLast"));
            Assert.That(user.Email, Is.EqualTo("old@test.com"));

            A.CallTo(() => _authRepo.EmailExistsAsync(A<string>._, A<CancellationToken>._))
                .MustNotHaveHappened();

            A.CallTo(() => _users.UpdateAsync(user)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            A.CallTo(() => _users.GetByIdAsync(userId)).Returns((User?)null);

            var cmd = new UpdateMyProfileCommand(userId, new UpdateMyProfileDto
            {
                FirstName = "New"
            });

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("not found").IgnoreCase);

            A.CallTo(() => _users.UpdateAsync(A<User>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_EmailProvided_AndAlreadyInUse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "old@test.com",
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);
            A.CallTo(() => _authRepo.EmailExistsAsync("taken@test.com", A<CancellationToken>._)).Returns(true);

            var cmd = new UpdateMyProfileCommand(userId, new UpdateMyProfileDto
            {
                Email = "taken@test.com"
            });

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("already in use").IgnoreCase);

            A.CallTo(() => _users.UpdateAsync(A<User>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_Should_NotCheckEmailUniqueness_When_EmailProvidedButUnchanged_IgnoringCase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "Test@Email.com",
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);

            var cmd = new UpdateMyProfileCommand(userId, new UpdateMyProfileDto
            {
                Email = "test@email.com" // casing only
            });

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);

            A.CallTo(() => _authRepo.EmailExistsAsync(A<string>._, A<CancellationToken>._))
                .MustNotHaveHappened();

            A.CallTo(() => _users.UpdateAsync(user)).MustHaveHappenedOnceExactly();
        }
    }
}
