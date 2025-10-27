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
        /// <param name="usernameOrEmail">Gebruikersnaam of email</param>
        /// <param name="password">Wachtwoord</param>
        /// <returns>Tuple met (isAuthenticated, userId). userId is null als authenticatie faalt</returns>
        Task<(bool isAuthenticated, string? userId)> AuthenticateAsync(string usernameOrEmail, string password);

        /// <summary>
        /// Genereer een JWT token voor een gebruiker
        /// </summary>
        /// <param name="usernameOrEmail">Gebruikersnaam of email</param>
        /// <param name="userId">User ID</param>
        /// <returns>JWT token</returns>
        string GenerateJwtToken(string usernameOrEmail, string userId);
    }
}
