using Application.Common.Interfaces;
using Application.Users.Commands.ChangeMyPassword;
using Application.Users.Dtos;
using Domain.Models.Entities.Users;
using FakeItEasy;

namespace Test.Application.Users
{
    [TestFixture]
    public class ChangeMyPasswordCommandHandlerTests
    {
        private IGenericRepository<User> _users = default!;
        private IPasswordHasher _passwordHasher = default!;
        private ChangeMyPasswordCommandHandler _sut = default!;

        [SetUp]
        public void SetUp()
        {
            _users = A.Fake<IGenericRepository<User>>();
            _passwordHasher = A.Fake<IPasswordHasher>();

            _sut = new ChangeMyPasswordCommandHandler(_users, _passwordHasher);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_When_CurrentPasswordValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                PasswordHash = "old-hash",
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);
            A.CallTo(() => _passwordHasher.Verify("current", "old-hash")).Returns(true);
            A.CallTo(() => _passwordHasher.Hash("newpass")).Returns("new-hash");

            var cmd = new ChangeMyPasswordCommand(userId, new ChangePasswordDto
            {
                CurrentPassword = "current",
                NewPassword = "newpass"
            });

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.True);
            Assert.That(user.PasswordHash, Is.EqualTo("new-hash"));

            A.CallTo(() => _users.UpdateAsync(user)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_CurrentPasswordInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                PasswordHash = "old-hash",
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);
            A.CallTo(() => _passwordHasher.Verify("wrong", "old-hash")).Returns(false);

            var cmd = new ChangeMyPasswordCommand(userId, new ChangePasswordDto
            {
                CurrentPassword = "wrong",
                NewPassword = "newpass"
            });

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("incorrect").IgnoreCase);

            A.CallTo(() => _users.UpdateAsync(A<User>._)).MustNotHaveHappened();
        }
    }
}
