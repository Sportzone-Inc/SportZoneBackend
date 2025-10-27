using NUnit.Framework;
using Moq;
using SportZone.Models;
using SportZone.Services;
using SportZone.Repositories;

namespace SportZone.Tests.Services;

[TestFixture]
public class EventServiceTests
{
    private Mock<IEventRepository> _mockEventRepository = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private IEventService _EventService = null!;

    [SetUp]
    public void Setup()
    {
        _mockEventRepository = new Mock<IEventRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _EventService = new EventService(_mockEventRepository.Object, _mockUserRepository.Object);
    }

    [Test]
    public async Task CreateEventAsync_ShouldCreateActivityWithUniqueId()
    {
        // Arrange
        var userId = "user123";
        var Event = new Event
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

        _mockEventRepository.Setup(x => x.CreateAsync(It.IsAny<Event>()))
            .ReturnsAsync(Event);

        // Act
        var result = await _EventService.CreateEventAsync(Event);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Evening Basketball"));
        Assert.That(result.CreatedBy, Is.EqualTo(userId));
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockEventRepository.Verify(x => x.CreateAsync(It.IsAny<Event>()), Times.Once);
    }

    [Test]
    public async Task GeteventsByUserAsync_ShouldReturnUserActivities()
    {
        // Arrange
        var userId = "user123";
        var activities = new List<Event>
        {
            new Event
            {
                Id = "activity1",
                Name = "Morning Run",
                SportType = SportType.Running,
                CreatedBy = userId
            },
            new Event
            {
                Id = "activity2",
                Name = "Evening Basketball",
                SportType = SportType.Basketball,
                CreatedBy = userId
            }
        };

        _mockEventRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(activities);

        // Act
        var result = await _EventService.GeteventsByUserAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(a => a.CreatedBy == userId), Is.True);
        _mockEventRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task GeteventsByTypeAsync_ShouldFilterBySportType()
    {
        // Arrange
        var sportType = SportType.Basketball;
        var activities = new List<Event>
        {
            new Event
            {
                Id = "activity1",
                Name = "Basketball Game 1",
                SportType = SportType.Basketball
            },
            new Event
            {
                Id = "activity2",
                Name = "Basketball Game 2",
                SportType = SportType.Basketball
            }
        };

        _mockEventRepository.Setup(x => x.GetBySportTypeAsync(sportType))
            .ReturnsAsync(activities);

        // Act
        var result = await _EventService.GeteventsByTypeAsync(sportType);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(a => a.SportType == SportType.Basketball), Is.True);
        _mockEventRepository.Verify(x => x.GetBySportTypeAsync(sportType), Times.Once);
    }
}
