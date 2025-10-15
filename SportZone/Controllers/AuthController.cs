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
        /// Login met gebruikersnaam en wachtwoord
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token bij succesvolle authenticatie</returns>
        /// <response code="200">Login succesvol, retourneert JWT token</response>
        /// <response code="401">Ongeldige credentials</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Username) || 
                string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new { message = "Gebruikersnaam en wachtwoord zijn verplicht" });
            }

            var isAuthenticated = await _authenticationService.AuthenticateAsync(
                loginRequest.Username, 
                loginRequest.Password
            );

            if (!isAuthenticated)
            {
                _logger.LogWarning("Mislukte login poging voor gebruiker: {Username}", loginRequest.Username);
                return Unauthorized(new { message = "Ongeldige gebruikersnaam of wachtwoord" });
            }

            var token = _authenticationService.GenerateJwtToken(loginRequest.Username);
            var expiresAt = DateTime.UtcNow.AddMinutes(60); // Default expiry

            _logger.LogInformation("Succesvolle login voor gebruiker: {Username}", loginRequest.Username);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Username = loginRequest.Username,
                ExpiresAt = expiresAt
            });
        }
    }
}
