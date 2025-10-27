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
        /// Login met gebruikersnaam of email en wachtwoord
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token en gebruikers informatie bij succesvolle authenticatie</returns>
        /// <response code="200">Login succesvol, retourneert JWT token en gebruikers informatie</response>
        /// <response code="400">Ongeldige input (beide velden ingevuld of beide leeg)</response>
        /// <response code="401">Ongeldige credentials</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            // Check if password is provided
            if (string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new { message = "Wachtwoord is verplicht" });
            }

            var hasUsername = !string.IsNullOrWhiteSpace(loginRequest.Username);
            var hasEmail = !string.IsNullOrWhiteSpace(loginRequest.Email);

            // Validate that exactly one field (Username OR Email) is filled
            if (!hasUsername && !hasEmail)
            {
                return BadRequest(new { message = "Gebruikersnaam of email is verplicht" });
            }

            if (hasUsername && hasEmail)
            {
                return BadRequest(new { message = "Vul alleen gebruikersnaam OF email in, niet beide" });
            }

            // Determine which field to use for authentication
            string identifier = hasUsername ? loginRequest.Username! : loginRequest.Email!;
            bool useEmail = hasEmail;

            var (isAuthenticated, userId) = await _authenticationService.AuthenticateAsync(
                identifier,
                loginRequest.Password,
                useEmail
            );

            if (!isAuthenticated || userId == null)
            {
                _logger.LogWarning("Mislukte login poging voor: {Identifier}", identifier);
                return Unauthorized(new { message = "Ongeldige gebruikersnaam/email of wachtwoord" });
            }

            var token = _authenticationService.GenerateJwtToken(identifier, userId);
            var expiresAt = DateTime.UtcNow.AddMinutes(60); // Default expiry

            _logger.LogInformation("Succesvolle login voor: {Identifier}", identifier);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Username = identifier,
                UserId = userId,
                ExpiresAt = expiresAt
            });
        }
    }
}
