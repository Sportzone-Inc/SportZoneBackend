using Microsoft.AspNetCore.Mvc;
using SportZone.DTOs;
using SportZone.Services;

namespace SportZone.Controllers
{
    /// <summary>
    /// Controller voor authenticatie
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthenticationService authenticationService,
            ILogger<AuthController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /// <summary>
        /// Login met gebruikersnaam/email en wachtwoord
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token en gebruikers informatie bij succesvolle authenticatie</returns>
        /// <response code="200">Login succesvol, retourneert JWT token en gebruikers informatie</response>
        /// <response code="401">Ongeldige credentials</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.UsernameOrEmail) || 
                string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new { message = "Gebruikersnaam/email en wachtwoord zijn verplicht" });
            }

            var (isAuthenticated, userId) = await _authenticationService.AuthenticateAsync(
                loginRequest.UsernameOrEmail, 
                loginRequest.Password
            );

            if (!isAuthenticated || userId == null)
            {
                _logger.LogWarning("Mislukte login poging voor gebruiker: {UsernameOrEmail}", loginRequest.UsernameOrEmail);
                return Unauthorized(new { message = "Ongeldige gebruikersnaam/email of wachtwoord" });
            }

            var token = _authenticationService.GenerateJwtToken(loginRequest.UsernameOrEmail, userId);
            var expiresAt = DateTime.UtcNow.AddMinutes(60); // Default expiry

            _logger.LogInformation("Succesvolle login voor gebruiker: {UsernameOrEmail}", loginRequest.UsernameOrEmail);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Username = loginRequest.UsernameOrEmail,
                UserId = userId,
                ExpiresAt = expiresAt
            });
        }
    }
}
