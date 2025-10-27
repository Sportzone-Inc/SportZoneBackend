using System.ComponentModel.DataAnnotations;

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
        /// <example>john_doe</example>
        public string? Username { get; set; }

        /// <summary>
        /// Email (optioneel als Username is ingevuld)
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string? Email { get; set; }

        /// <summary>
        /// Wachtwoord (verplicht)
        /// </summary>
        /// <example>MySecurePassword123!</example>
        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        public string Password { get; set; } = string.Empty;
    }
}
