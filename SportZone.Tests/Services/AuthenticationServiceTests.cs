using NUnit.Framework;
using Moq;
using SportZone.Repositories;
using Microsoft.Extensions.Configuration;
using SportZone.Authentication;

namespace SportZone.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IConfiguration> _mockConfiguration;
        private AuthenticationService _authenticationService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration mock
            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(x => x["SecretKey"]).Returns("Test-Secret-Key-For-JWT-Testing-Minimum-32-Characters-Long");
            jwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
            jwtSection.Setup(x => x["Audience"]).Returns("TestAudience");
            jwtSection.Setup(x => x["ExpiryMinutes"]).Returns("60");
            
            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);

            //_authenticationService = new AuthenticationService(
            //    _mockUserRepository.Object,
            //    _mockConfiguration.Object
            //);
        }

        [Test]
        public async Task AuthenticateAsync_WithValidCredentials_ReturnsTrue()
        {
            // Arrange
            var username = "admin";
            var password = "passtimadmin";

            _mockUserRepository
                .Setup(x => x.VerifyPasswordAsync(username, password))
                .ReturnsAsync(true);

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.True);
            _mockUserRepository.Verify(x => x.VerifyPasswordAsync(username, password), Times.Once);
        }

        [Test]
        public async Task AuthenticateAsync_WithInvalidUsername_ReturnsFalse()
        {
            // Arrange
            var username = "invaliduser";
            var password = "password";

            _mockUserRepository
                .Setup(x => x.VerifyPasswordAsync(username, password))
                .ReturnsAsync(false);

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.VerifyPasswordAsync(username, password), Times.Once);
        }

        [Test]
        public async Task AuthenticateAsync_WithInvalidPassword_ReturnsFalse()
        {
            // Arrange
            var username = "admin";
            var password = "wrongpassword";

            _mockUserRepository
                .Setup(x => x.VerifyPasswordAsync(username, password))
                .ReturnsAsync(false);

            // Act
            var result = await _authenticationService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
            _mockUserRepository.Verify(x => x.VerifyPasswordAsync(username, password), Times.Once);
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
