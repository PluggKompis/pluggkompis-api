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
        public async Task Handle_Should_ReturnSuccess_When_UserExists_AndEmailNotTaken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@test.com",
                IsActive = true
            };

            var dto = new UpdateMyProfileDto
            {
                FirstName = "New",
                LastName = "Name",
                Email = "new@test.com"
            };

            var mappedDto = new MyProfileDto
            {
                Id = userId,
                FirstName = "New",
                LastName = "Name",
                Email = "new@test.com",
                Role = "Student",
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);
            A.CallTo(() => _authRepo.EmailExistsAsync("new@test.com", A<CancellationToken>._)).Returns(false);
            A.CallTo(() => _mapper.Map<MyProfileDto>(A<User>._)).Returns(mappedDto);

            var cmd = new UpdateMyProfileCommand(userId, dto);

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Email, Is.EqualTo("new@test.com"));

            Assert.That(user.FirstName, Is.EqualTo("New"));
            Assert.That(user.LastName, Is.EqualTo("Name"));
            Assert.That(user.Email, Is.EqualTo("new@test.com"));

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
                FirstName = "New",
                LastName = "Name",
                Email = "new@test.com"
            });

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("not found").IgnoreCase);

            A.CallTo(() => _users.UpdateAsync(A<User>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_EmailAlreadyInUse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@test.com",
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);
            A.CallTo(() => _authRepo.EmailExistsAsync("taken@test.com", A<CancellationToken>._)).Returns(true);

            var cmd = new UpdateMyProfileCommand(userId, new UpdateMyProfileDto
            {
                FirstName = "New",
                LastName = "Name",
                Email = "taken@test.com"
            });

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("already in use").IgnoreCase);

            A.CallTo(() => _users.UpdateAsync(A<User>._)).MustNotHaveHappened();
        }
    }
}
