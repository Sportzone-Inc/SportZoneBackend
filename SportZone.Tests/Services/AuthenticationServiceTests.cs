using NUnit.Framework;
using Moq;
using SportZone.Services;
using SportZone.Repositories;
using SportZone.Models;
using SportZone.DTOs;

namespace SportZone.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private AuthenticationService _authenticationService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _authenticationService = new AuthenticationService(
                _mockUserRepository.Object,
                _mockPasswordHasher.Object
            );
        }

        [Test]
        public async Task AuthenticateAsync_WithValidCredentials_ReturnsTrue()
        {
            // Arrange
            var username = "admin";
            var password = "passtimadmin";
            var hashedPassword = "hashed_password";
            
            var user = new User
            {
                Id = "1",
                Username = username,
                PasswordHash = hashedPassword,
                Email = "admin@sportzone.com",
                FirstName = "Admin",
                LastName = "User"
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);
            
            _mockPasswordHasher
                .Setup(x => x.VerifyPassword(password, hashedPassword))
                .Returns(true);

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.True);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(username), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(password, hashedPassword), Times.Once);
        }

        [Test]
        public async Task AuthenticateAsync_WithInvalidUsername_ReturnsFalse()
        {
            // Arrange
            var username = "invaliduser";
            var password = "password";

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(username), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithInvalidPassword_ReturnsFalse()
        {
            // Arrange
            var username = "admin";
            var password = "wrongpassword";
            var hashedPassword = "hashed_password";
            
            var user = new User
            {
                Id = "1",
                Username = username,
                PasswordHash = hashedPassword,
                Email = "admin@sportzone.com",
                FirstName = "Admin",
                LastName = "User"
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);
            
            _mockPasswordHasher
                .Setup(x => x.VerifyPassword(password, hashedPassword))
                .Returns(false);

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(username), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(password, hashedPassword), Times.Once);
        }
    }
}
