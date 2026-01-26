using Application.Common.Interfaces;
using Application.Users.Commands.SoftDeleteMe;
using Domain.Models.Entities.Users;
using FakeItEasy;

namespace Test.Application.Users
{
    [TestFixture]
    public class SoftDeleteMeCommandHandlerTests
    {
        private IGenericRepository<User> _users = default!;
        private SoftDeleteMeCommandHandler _sut = default!;

        [SetUp]
        public void SetUp()
        {
            _users = A.Fake<IGenericRepository<User>>();
            _sut = new SoftDeleteMeCommandHandler(_users);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_And_SetIsActiveFalse_When_UserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);

            var cmd = new SoftDeleteMeCommand(userId);

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(user.IsActive, Is.False);

            A.CallTo(() => _users.UpdateAsync(user)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            A.CallTo(() => _users.GetByIdAsync(userId)).Returns((User?)null);

            var cmd = new SoftDeleteMeCommand(userId);

            // Act
            var result = await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("not found").IgnoreCase);

            A.CallTo(() => _users.UpdateAsync(A<User>._)).MustNotHaveHappened();
        }
    }
}
