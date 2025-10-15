using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SportZone.Repositories;

namespace SportZone.Services
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
        /// Authenticeer een gebruiker met gebruikersnaam en wachtwoord
        /// </summary>
        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            // Get user from repository
            var user = await _userRepository.GetByUsernameAsync(username);
            
            // Check if user exists
            if (user == null)
            {
                return false;
            }

            // Check if password hash exists
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return false;
            }

            // Verify password
            return _passwordHasher.VerifyPassword(password, user.PasswordHash);
        }

        /// <summary>
        /// Genereer een JWT token voor een gebruiker
        /// </summary>
        public string GenerateJwtToken(string username)
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
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Sub, username),
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
