namespace SportZone.DTOs
{
    /// <summary>
    /// Data Transfer Object voor login aanvraag
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Gebruikersnaam
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Wachtwoord
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
