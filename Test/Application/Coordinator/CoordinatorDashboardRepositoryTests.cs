using Domain.Models.Entities.Venues;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Test.Application.Coordinator
{
    [TestFixture]
    public class CoordinatorDashboardRepositoryTests
    {
        [Test]
        public async Task Calculates_Total_Volunteers_Correctly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new AppDbContext(options);

            var coordinatorId = Guid.NewGuid();
            var venueId = Guid.NewGuid();

            db.Venues.Add(new Venue
            {
                Id = venueId,
                CoordinatorId = coordinatorId,
                Name = "Unit Test Venue",
                Address = "Test",
                City = "City",
                PostalCode = "123",
                Description = "Desc",
                ContactEmail = "x@test.se",
                ContactPhone = "123",
                IsActive = true
            });

            db.VolunteerApplications.AddRange(
                new VolunteerApplication
                {
                    VolunteerId = Guid.NewGuid(),
                    VenueId = venueId,
                    Status = VolunteerApplicationStatus.Approved
                },
                new VolunteerApplication
                {
                    VolunteerId = Guid.NewGuid(),
                    VenueId = venueId,
                    Status = VolunteerApplicationStatus.Approved
                }
            );

            await db.SaveChangesAsync();

            var repo = new CoordinatorDashboardRepository(db);

            // Act
            var result = await repo.BuildCoordinatorDashboardAsync(coordinatorId, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.TotalVolunteers, Is.EqualTo(2));
        }
    }
}
