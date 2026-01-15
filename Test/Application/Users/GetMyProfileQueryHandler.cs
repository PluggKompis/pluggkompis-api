using Application.Common.Interfaces;
using Application.Users.Dtos;
using Application.Users.Queries.GetMyProfile;
using AutoMapper;
using Domain.Models.Entities.Users;
using FakeItEasy;

namespace Test.Application.Users
{
    [TestFixture]
    public class GetMyProfileQueryHandlerTests
    {
        private IGenericRepository<User> _users = default!;
        private IMapper _mapper = default!;
        private GetMyProfileQueryHandler _sut = default!;

        [SetUp]
        public void SetUp()
        {
            _users = A.Fake<IGenericRepository<User>>();
            _mapper = A.Fake<IMapper>();

            _sut = new GetMyProfileQueryHandler(_users, _mapper);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_When_UserExists_AndIsActive()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FirstName = "Mohanned",
                LastName = "Test",
                Email = "mohanned@test.com",
                IsActive = true
            };

            var mapped = new MyProfileDto
            {
                Id = userId,
                FirstName = "Mohanned",
                LastName = "Test",
                Email = "mohanned@test.com",
                Role = "Student",
                IsActive = true
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);
            A.CallTo(() => _mapper.Map<MyProfileDto>(user)).Returns(mapped);

            var query = new GetMyProfileQuery(userId);

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Id, Is.EqualTo(userId));

            A.CallTo(() => _users.GetByIdAsync(userId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _mapper.Map<MyProfileDto>(user)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            A.CallTo(() => _users.GetByIdAsync(userId)).Returns((User?)null);

            var query = new GetMyProfileQuery(userId);

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("not found").IgnoreCase);
            Assert.That(result.Data, Is.Null);

            A.CallTo(() => _mapper.Map<MyProfileDto>(A<User>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UserIsInactive()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                IsActive = false
            };

            A.CallTo(() => _users.GetByIdAsync(userId)).Returns(user);

            var query = new GetMyProfileQuery(userId);

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("not found").IgnoreCase);
            Assert.That(result.Data, Is.Null);

            A.CallTo(() => _mapper.Map<MyProfileDto>(A<User>._)).MustNotHaveHappened();
        }
    }
}
