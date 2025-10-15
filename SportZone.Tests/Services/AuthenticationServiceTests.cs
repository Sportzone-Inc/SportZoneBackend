using NUnit.Framework;
using Moq;
using SportZone.Services;
using SportZone.Repositories;
using SportZone.Models;
using Microsoft.Extensions.Configuration;
using SportZone.Authentication;

namespace SportZone.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private Mock<IConfiguration> _mockConfiguration;
        private AuthenticationService _authenticationService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration mock
            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(x => x["SecretKey"]).Returns("Test-Secret-Key-For-JWT-Testing-Minimum-32-Characters-Long");
            jwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
            jwtSection.Setup(x => x["Audience"]).Returns("TestAudience");
            jwtSection.Setup(x => x["ExpiryMinutes"]).Returns("60");
            
            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);

            _authenticationService = new AuthenticationService(
                _mockUserRepository.Object,
                _mockPasswordHasher.Object,
                _mockConfiguration.Object
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
                LastName = "User",
                Name = "Admin User"
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
                LastName = "User",
                Name = "Admin User"
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

        [Test]
        public async Task AuthenticateAsync_WithEmptyUsername_ReturnsFalse()
        {
            // Arrange
            var username = "";
            var password = "password";

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(It.IsAny<string>()), Times.Never);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var username = "admin";
            var password = "";

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(It.IsAny<string>()), Times.Never);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithNullUsername_ReturnsFalse()
        {
            // Arrange
            string? username = null;
            var password = "password";

            // Act
            var result = await _authenticationService.AuthenticateAsync(username!, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(It.IsAny<string>()), Times.Never);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithNullPassword_ReturnsFalse()
        {
            // Arrange
            var username = "admin";
            string? password = null;

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password!);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(It.IsAny<string>()), Times.Never);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithUserHavingNullPasswordHash_ReturnsFalse()
        {
            // Arrange
            var username = "admin";
            var password = "password";
            
            var user = new User
            {
                Id = "1",
                Username = username,
                PasswordHash = null!,
                Email = "admin@sportzone.com",
                FirstName = "Admin",
                LastName = "User",
                Name = "Admin User"
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(username), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithUserHavingEmptyPasswordHash_ReturnsFalse()
        {
            // Arrange
            var username = "admin";
            var password = "password";
            
            var user = new User
            {
                Id = "1",
                Username = username,
                PasswordHash = "",
                Email = "admin@sportzone.com",
                FirstName = "Admin",
                LastName = "User",
                Name = "Admin User"
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(username), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GenerateJwtToken_WithValidUsername_ReturnsToken()
        {
            // Arrange
            var username = "admin";

            // Act
            var token = _authenticationService.GenerateJwtToken(username);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
            Assert.That(token.Split('.').Length, Is.EqualTo(3)); // JWT has 3 parts
        }
    }
}
