namespace SportZone.DTOs
{
    /// <summary>
    /// Data Transfer Object voor login aanvraag
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Gebruikersnaam (optioneel als Email is ingevuld)
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Email (optioneel als Username is ingevuld)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Wachtwoord (verplicht)
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
