using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SportZone.Repositories;
using SportZone.Services;

namespace SportZone.Authentication
{
    /// <summary>
    /// Service voor authenticatie functies
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticeer een gebruiker met gebruikersnaam of email en wachtwoord
        /// </summary>
        public async Task<(bool isAuthenticated, string? userId)> AuthenticateAsync(string identifier, string password, bool isEmail)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
            {
                return (false, null);
            }

            // Get user based on whether identifier is email or username
            var user = isEmail 
                ? await _userRepository.GetByEmailAsync(identifier)
                : await _userRepository.GetByUsernameAsync(identifier);
            
            // Check if user exists
            if (user == null)
            {
                return (false, null);
            }

            // Check if password hash exists
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return (false, null);
            }

            // Verify password
            var isPasswordValid = _passwordHasher.VerifyPassword(password, user.Password);
            
            if (isPasswordValid)
            {
                return (true, user.Id);
            }

            return (false, null);
        }

        /// <summary>
        /// Genereer een JWT token voor een gebruiker
        /// </summary>
        public string GenerateJwtToken(string identifier, string userId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey niet geconfigureerd");
            var issuer = jwtSettings["Issuer"] ?? "SportZone";
            var audience = jwtSettings["Audience"] ?? "SportZoneUsers";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, identifier),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Sub, identifier),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
