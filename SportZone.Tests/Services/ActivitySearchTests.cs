using Moq;
using NUnit.Framework;
using SportZone.Models;
using SportZone.Repositories;
using SportZone.Services;

namespace SportZone.Tests.Services;

[TestFixture]
public class ActivitySearchTests
{
    private Mock<ISportActivityRepository> _mockActivityRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private SportActivityService _service;

    [SetUp]
    public void Setup()
    {
        _mockActivityRepository = new Mock<ISportActivityRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _service = new SportActivityService(_mockActivityRepository.Object, _mockUserRepository.Object);
    }

    [Test]
    public async Task SearchActivitiesByLocation_WithinRadius_ReturnsMatchingActivities()
    {
        // Arrange
        var userLatitude = 51.0543;  // Gent, Belgium
        var userLongitude = 3.7174;
        var radiusKm = 10.0;

        var activities = new List<SportActivity>
        {
            new SportActivity
            {
                Id = "1",
                Name = "Basketball Near Center",
                SportType = SportType.Basketball,
                Latitude = 51.0500,
                Longitude = 3.7200,
                CreatedBy = "user1",
                IsActive = true
            },
            new SportActivity
            {
                Id = "2",
                Name = "Basketball Far Away",
                SportType = SportType.Basketball,
                Latitude = 52.0000,  // Too far
                Longitude = 4.0000,
                CreatedBy = "user2",
                IsActive = true
            }
        };

        _mockActivityRepository.Setup(repo => repo.GetActiveActivitiesAsync())
            .ReturnsAsync(activities);

        // Act
        var result = await _service.SearchActivitiesByLocationAsync(userLatitude, userLongitude, radiusKm);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo("1"));
    }

    [Test]
    public async Task SearchActivitiesByDateRange_ReturnsActivitiesInRange()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(7);

        var activities = new List<SportActivity>
        {
            new SportActivity
            {
                Id = "1",
                Name = "Basketball This Week",
                SportType = SportType.Basketball,
                ScheduledDate = DateTime.UtcNow.AddDays(3),
                CreatedBy = "user1",
                IsActive = true
            },
            new SportActivity
            {
                Id = "2",
                Name = "Basketball Next Month",
                SportType = SportType.Basketball,
                ScheduledDate = DateTime.UtcNow.AddDays(30),
                CreatedBy = "user2",
                IsActive = true
            }
        };

        _mockActivityRepository.Setup(repo => repo.GetActivitiesByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(new List<SportActivity> { activities[0] });

        // Act
        var result = await _service.SearchActivitiesByDateRangeAsync(startDate, endDate);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo("1"));
    }

    [Test]
    public async Task SearchActivitiesWithFilters_CombinedFilters_ReturnsMatchingActivities()
    {
        // Arrange
        var sportType = SportType.Basketball;
        var userLatitude = 51.0543;
        var userLongitude = 3.7174;
        var radiusKm = 10.0;
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(7);

        var activities = new List<SportActivity>
        {
            new SportActivity
            {
                Id = "1",
                Name = "Basketball Match",
                SportType = SportType.Basketball,
                Latitude = 51.0500,
                Longitude = 3.7200,
                ScheduledDate = DateTime.UtcNow.AddDays(3),
                MaxParticipants = 10,
                CurrentParticipants = 5,
                CreatedBy = "user1",
                IsActive = true
            },
            new SportActivity
            {
                Id = "2",
                Name = "Running Event",
                SportType = SportType.Running,
                Latitude = 51.0500,
                Longitude = 3.7200,
                ScheduledDate = DateTime.UtcNow.AddDays(3),
                CreatedBy = "user2",
                IsActive = true
            }
        };

        _mockActivityRepository.Setup(repo => repo.GetBySportTypeAsync(sportType))
            .ReturnsAsync(new List<SportActivity> { activities[0] });

        // Act
        var result = await _service.SearchActivitiesWithFiltersAsync(
            sportType: sportType,
            latitude: userLatitude,
            longitude: userLongitude,
            radiusKm: radiusKm,
            startDate: startDate,
            endDate: endDate,
            hasAvailableSlots: true
        );

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().SportType, Is.EqualTo(SportType.Basketball));
    }
}
