using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using SportZone.Models;
using SportZone.Repositories;

namespace SportZone.Tests.Repositories;

[TestFixture]
public class EquipmentRepositoryTests
{
    private Mock<IMongoCollection<Equipment>> _mockCollection = null!;
    private Mock<IMongoDatabase> _mockDatabase = null!;
    private EquipmentRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        _mockCollection = new Mock<IMongoCollection<Equipment>>();
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockDatabase.Setup(db => db.GetCollection<Equipment>("equipment", null))
            .Returns(_mockCollection.Object);
        _repository = new EquipmentRepository(_mockDatabase.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldInsertEquipmentAndReturnIt()
    {
        // Arrange
        var equipment = new Equipment
        {
            Name = "Basketball",
            Type = EquipmentType.Ball,
            Category = EquipmentCategory.Sports,
            Description = "Official size basketball",
            IsActive = true
        };

        _mockCollection.Setup(c => c.InsertOneAsync(
            It.IsAny<Equipment>(),
            null,
            default))
            .Returns(Task.CompletedTask)
            .Callback<Equipment, InsertOneOptions, CancellationToken>((e, _, _) => e.Id = "test-id");

        // Act
        var result = await _repository.CreateAsync(equipment);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Basketball"));
        Assert.That(result.Type, Is.EqualTo(EquipmentType.Ball));
        _mockCollection.Verify(c => c.InsertOneAsync(
            It.IsAny<Equipment>(),
            null,
            default), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenEquipmentExists_ShouldReturnEquipment()
    {
        // Arrange
        var equipment = new Equipment
        {
            Id = "test-id",
            Name = "Tennis Racket",
            Type = EquipmentType.Racket,
            IsActive = true
        };

        var mockCursor = new Mock<IAsyncCursor<Equipment>>();
        mockCursor.Setup(_ => _.Current).Returns(new List<Equipment> { equipment });
        mockCursor
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        mockCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Equipment>>(),
            It.IsAny<FindOptions<Equipment, Equipment>>(),
            default))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetByIdAsync("test-id");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("test-id"));
        Assert.That(result.Name, Is.EqualTo("Tennis Racket"));
    }

    [Test]
    public async Task GetByIdAsync_WhenEquipmentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var mockCursor = new Mock<IAsyncCursor<Equipment>>();
        mockCursor.Setup(_ => _.Current).Returns(new List<Equipment>());
        mockCursor
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        mockCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Equipment>>(),
            It.IsAny<FindOptions<Equipment, Equipment>>(),
            default))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetByIdAsync("non-existent-id");

        // Assert
        Assert.That(result, Is.Null);
    }
}
