namespace SportZone.Services
{
    /// <summary>
    /// Interface voor authenticatie service
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticeer een gebruiker met gebruikersnaam en wachtwoord
        /// </summary>
        /// <param name="username">Gebruikersnaam</param>
        /// <param name="password">Wachtwoord</param>
        /// <returns>True als authenticatie succesvol is, anders false</returns>
        Task<bool> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Genereer een JWT token voor een gebruiker
        /// </summary>
        /// <param name="username">Gebruikersnaam</param>
        /// <returns>JWT token</returns>
        string GenerateJwtToken(string username);
    }
}
