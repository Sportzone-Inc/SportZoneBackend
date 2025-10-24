using Moq;
using NUnit.Framework;
using SportZone.Models;
using SportZone.Repositories;
using SportZone.Services;

namespace SportZone.Tests.Services;

[TestFixture]
public class ActivityParticipationTests
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
    public async Task JoinSportActivityAsync_ValidUser_ReturnsTrue()
    {
        // Arrange
        var activityId = "507f1f77bcf86cd799439011";
        var userId = "507f1f77bcf86cd799439012";
        var activity = new SportActivity
        {
            Id = activityId,
            Name = "Basketball Game",
            SportType = SportType.Basketball,
            MaxParticipants = 10,
            CurrentParticipants = 5,
            Participants = new List<string>(),
            CreatedBy = "507f1f77bcf86cd799439013",
            IsActive = true
        };

        _mockActivityRepository.Setup(repo => repo.GetByIdAsync(activityId))
            .ReturnsAsync(activity);
        _mockActivityRepository.Setup(repo => repo.JoinActivityAsync(activityId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.JoinSportActivityAsync(activityId, userId);

        // Assert
        Assert.That(result, Is.True);
        _mockActivityRepository.Verify(repo => repo.JoinActivityAsync(activityId, userId), Times.Once);
    }

    [Test]
    public void JoinSportActivityAsync_ActivityFull_ThrowsInvalidOperationException()
    {
        // Arrange
        var activityId = "507f1f77bcf86cd799439011";
        var userId = "507f1f77bcf86cd799439012";
        var activity = new SportActivity
        {
            Id = activityId,
            Name = "Basketball Game",
            SportType = SportType.Basketball,
            MaxParticipants = 5,
            CurrentParticipants = 5,
            Participants = new List<string> { "user1", "user2", "user3", "user4", "user5" },
            CreatedBy = "507f1f77bcf86cd799439013",
            IsActive = true
        };

        _mockActivityRepository.Setup(repo => repo.GetByIdAsync(activityId))
            .ReturnsAsync(activity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.JoinSportActivityAsync(activityId, userId));
        Assert.That(ex.Message, Is.EqualTo("Activity is full"));
    }

    [Test]
    public void JoinSportActivityAsync_UserAlreadyJoined_ThrowsInvalidOperationException()
    {
        // Arrange
        var activityId = "507f1f77bcf86cd799439011";
        var userId = "507f1f77bcf86cd799439012";
        var activity = new SportActivity
        {
            Id = activityId,
            Name = "Basketball Game",
            SportType = SportType.Basketball,
            MaxParticipants = 10,
            CurrentParticipants = 3,
            Participants = new List<string> { "user1", userId, "user3" },
            CreatedBy = "507f1f77bcf86cd799439013",
            IsActive = true
        };

        _mockActivityRepository.Setup(repo => repo.GetByIdAsync(activityId))
            .ReturnsAsync(activity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.JoinSportActivityAsync(activityId, userId));
        Assert.That(ex.Message, Is.EqualTo("User already joined this activity"));
    }

    [Test]
    public async Task LeaveSportActivityAsync_ValidUser_ReturnsTrue()
    {
        // Arrange
        var activityId = "507f1f77bcf86cd799439011";
        var userId = "507f1f77bcf86cd799439012";
        var activity = new SportActivity
        {
            Id = activityId,
            Name = "Basketball Game",
            SportType = SportType.Basketball,
            MaxParticipants = 10,
            CurrentParticipants = 3,
            Participants = new List<string> { "user1", userId, "user3" },
            CreatedBy = "507f1f77bcf86cd799439013",
            IsActive = true
        };

        _mockActivityRepository.Setup(repo => repo.GetByIdAsync(activityId))
            .ReturnsAsync(activity);
        _mockActivityRepository.Setup(repo => repo.LeaveActivityAsync(activityId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.LeaveSportActivityAsync(activityId, userId);

        // Assert
        Assert.That(result, Is.True);
        _mockActivityRepository.Verify(repo => repo.LeaveActivityAsync(activityId, userId), Times.Once);
    }

    [Test]
    public void LeaveSportActivityAsync_UserNotParticipant_ThrowsInvalidOperationException()
    {
        // Arrange
        var activityId = "507f1f77bcf86cd799439011";
        var userId = "507f1f77bcf86cd799439012";
        var activity = new SportActivity
        {
            Id = activityId,
            Name = "Basketball Game",
            SportType = SportType.Basketball,
            MaxParticipants = 10,
            CurrentParticipants = 2,
            Participants = new List<string> { "user1", "user3" },
            CreatedBy = "507f1f77bcf86cd799439013",
            IsActive = true
        };

        _mockActivityRepository.Setup(repo => repo.GetByIdAsync(activityId))
            .ReturnsAsync(activity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.LeaveSportActivityAsync(activityId, userId));
        Assert.That(ex.Message, Is.EqualTo("User is not part of this activity"));
    }

    [Test]
    public async Task JoinSportActivityAsync_ActivityWithNoMaxParticipants_ReturnsTrue()
    {
        // Arrange
        var activityId = "507f1f77bcf86cd799439011";
        var userId = "507f1f77bcf86cd799439012";
        var activity = new SportActivity
        {
            Id = activityId,
            Name = "Running Group",
            SportType = SportType.Running,
            MaxParticipants = null, // No limit
            CurrentParticipants = 15,
            Participants = new List<string>(),
            CreatedBy = "507f1f77bcf86cd799439013",
            IsActive = true
        };

        _mockActivityRepository.Setup(repo => repo.GetByIdAsync(activityId))
            .ReturnsAsync(activity);
        _mockActivityRepository.Setup(repo => repo.JoinActivityAsync(activityId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.JoinSportActivityAsync(activityId, userId);

        // Assert
        Assert.That(result, Is.True);
        _mockActivityRepository.Verify(repo => repo.JoinActivityAsync(activityId, userId), Times.Once);
    }
}
