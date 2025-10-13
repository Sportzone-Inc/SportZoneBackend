using NUnit.Framework;
using Moq;
using SportZone.Models;
using SportZone.Services;
using SportZone.Repositories;

namespace SportZone.Tests.Services;

[TestFixture]
public class SportActivityServiceTests
{
    private Mock<ISportActivityRepository> _mockSportActivityRepository = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private ISportActivityService _sportActivityService = null!;

    [SetUp]
    public void Setup()
    {
        _mockSportActivityRepository = new Mock<ISportActivityRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _sportActivityService = new SportActivityService(_mockSportActivityRepository.Object, _mockUserRepository.Object);
    }

    [Test]
    public async Task CreateSportActivityAsync_ShouldCreateActivityWithUniqueId()
    {
        // Arrange
        var userId = "user123";
        var sportActivity = new SportActivity
        {
            Name = "Evening Basketball",
            SportType = SportType.Basketball,
            Location = "Gent Sports Center",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            MaxParticipants = 10,
            CreatedBy = userId
        };

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            PreferredSport = "Basketball"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockSportActivityRepository.Setup(x => x.CreateAsync(It.IsAny<SportActivity>()))
            .ReturnsAsync(sportActivity);

        // Act
        var result = await _sportActivityService.CreateSportActivityAsync(sportActivity);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Evening Basketball"));
        Assert.That(result.CreatedBy, Is.EqualTo(userId));
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockSportActivityRepository.Verify(x => x.CreateAsync(It.IsAny<SportActivity>()), Times.Once);
    }

    [Test]
    public async Task GetSportActivitiesByUserAsync_ShouldReturnUserActivities()
    {
        // Arrange
        var userId = "user123";
        var activities = new List<SportActivity>
        {
            new SportActivity
            {
                Id = "activity1",
                Name = "Morning Run",
                SportType = SportType.Running,
                CreatedBy = userId
            },
            new SportActivity
            {
                Id = "activity2",
                Name = "Evening Basketball",
                SportType = SportType.Basketball,
                CreatedBy = userId
            }
        };

        _mockSportActivityRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(activities);

        // Act
        var result = await _sportActivityService.GetSportActivitiesByUserAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(a => a.CreatedBy == userId), Is.True);
        _mockSportActivityRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task GetSportActivitiesByTypeAsync_ShouldFilterBySportType()
    {
        // Arrange
        var sportType = SportType.Basketball;
        var activities = new List<SportActivity>
        {
            new SportActivity
            {
                Id = "activity1",
                Name = "Basketball Game 1",
                SportType = SportType.Basketball
            },
            new SportActivity
            {
                Id = "activity2",
                Name = "Basketball Game 2",
                SportType = SportType.Basketball
            }
        };

        _mockSportActivityRepository.Setup(x => x.GetBySportTypeAsync(sportType))
            .ReturnsAsync(activities);

        // Act
        var result = await _sportActivityService.GetSportActivitiesByTypeAsync(sportType);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(a => a.SportType == SportType.Basketball), Is.True);
        _mockSportActivityRepository.Verify(x => x.GetBySportTypeAsync(sportType), Times.Once);
    }
}
