using NUnit.Framework;
using Moq;
using SportZone.Models;
using SportZone.Services;
using SportZone.Repositories;

namespace SportZone.Tests.Services;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IPasswordHasher> _mockPasswordHasher = null!;
    private IUserService _userService = null!;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _userService = new UserService(_mockUserRepository.Object, _mockPasswordHasher.Object);
    }

    [Test]
    public async Task CreateUserAsync_ShouldHashPasswordAndCreateUser()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            Password = "plainPassword123",
            Name = "Test User",
            PreferredSport = "Basketball"
        };

        var hashedPassword = "hashed_password_123";
        _mockPasswordHasher.Setup(x => x.HashPassword(user.Password))
            .Returns(hashedPassword);

        _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.CreateUserAsync(user);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(user.Email));
        _mockPasswordHasher.Verify(x => x.HashPassword("plainPassword123"), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.Is<User>(u => u.Password == hashedPassword)), Times.Once);
    }

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = "123456";
        var expectedUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            PreferredSport = "Running"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(userId));
        Assert.That(result.Email, Is.EqualTo(expectedUser.Email));
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task UpdateUserAsync_ShouldHashNewPasswordAndUpdateUser()
    {
        // Arrange
        var userId = "123456";
        var updatedUser = new User
        {
            Id = userId,
            Email = "updated@example.com",
            Password = "newPassword123",
            Name = "Updated User",
            PreferredSport = "Running"
        };

        var hashedPassword = "hashed_new_password";
        _mockPasswordHasher.Setup(x => x.HashPassword(updatedUser.Password))
            .Returns(hashedPassword);

        _mockUserRepository.Setup(x => x.UpdateAsync(userId, It.IsAny<User>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserAsync(userId, updatedUser);

        // Assert
        Assert.That(result, Is.True);
        _mockPasswordHasher.Verify(x => x.HashPassword("newPassword123"), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateAsync(userId, It.Is<User>(u => u.Password == hashedPassword)), Times.Once);
    }
}
