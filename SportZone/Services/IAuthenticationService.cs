namespace SportZone.Services
{
    /// <summary>
    /// Interface voor authenticatie service
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticeer een gebruiker met gebruikersnaam of email en wachtwoord
        /// </summary>
        /// <param name="identifier">Gebruikersnaam of email</param>
        /// <param name="password">Wachtwoord</param>
        /// <param name="isEmail">True als identifier een email is, false als het een gebruikersnaam is</param>
        /// <returns>Tuple met (isAuthenticated, userId). userId is null als authenticatie faalt</returns>
        Task<(bool isAuthenticated, string? userId)> AuthenticateAsync(string identifier, string password, bool isEmail);

        /// <summary>
        /// Genereer een JWT token voor een gebruiker
        /// </summary>
        /// <param name="identifier">Gebruikersnaam of email</param>
        /// <param name="userId">User ID</param>
        /// <returns>JWT token</returns>
        string GenerateJwtToken(string identifier, string userId);
    }
}
