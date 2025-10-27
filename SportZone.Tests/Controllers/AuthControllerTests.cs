using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SportZone.Controllers;
using SportZone.DTOs;
using SportZone.Services;

namespace SportZone.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthenticationService> _mockAuthService;
        private Mock<ILogger<AuthController>> _mockLogger;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Login_WithValidUsername_ReturnsOkWithTokenAndUserId()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                UsernameOrEmail = "testuser",
                Password = "password123"
            };

            var expectedUserId = "507f1f77bcf86cd799439011";
            var expectedToken = "mock-jwt-token";

            _mockAuthService
                .Setup(x => x.AuthenticateAsync(loginRequest.UsernameOrEmail, loginRequest.Password))
                .ReturnsAsync((true, expectedUserId));

            _mockAuthService
                .Setup(x => x.GenerateJwtToken(loginRequest.UsernameOrEmail, expectedUserId))
                .Returns(expectedToken);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            
            var response = okResult!.Value as LoginResponseDto;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Token, Is.EqualTo(expectedToken));
            Assert.That(response.UserId, Is.EqualTo(expectedUserId));
            Assert.That(response.Username, Is.EqualTo(loginRequest.UsernameOrEmail));
        }

        [Test]
        public async Task Login_WithValidEmail_ReturnsOkWithTokenAndUserId()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                UsernameOrEmail = "test@example.com",
                Password = "password123"
            };

            var expectedUserId = "507f1f77bcf86cd799439011";
            var expectedToken = "mock-jwt-token";

            _mockAuthService
                .Setup(x => x.AuthenticateAsync(loginRequest.UsernameOrEmail, loginRequest.Password))
                .ReturnsAsync((true, expectedUserId));

            _mockAuthService
                .Setup(x => x.GenerateJwtToken(loginRequest.UsernameOrEmail, expectedUserId))
                .Returns(expectedToken);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            
            var response = okResult!.Value as LoginResponseDto;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Token, Is.EqualTo(expectedToken));
            Assert.That(response.UserId, Is.EqualTo(expectedUserId));
            Assert.That(response.Username, Is.EqualTo(loginRequest.UsernameOrEmail));
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                UsernameOrEmail = "invaliduser",
                Password = "wrongpassword"
            };

            _mockAuthService
                .Setup(x => x.AuthenticateAsync(loginRequest.UsernameOrEmail, loginRequest.Password))
                .ReturnsAsync((false, null));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }
    }
}
