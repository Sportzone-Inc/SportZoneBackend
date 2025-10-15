namespace SportZone.DTOs
{
    /// <summary>
    /// Data Transfer Object voor login response
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT token voor authenticatie
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gebruikersnaam
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Expiratie datum van de token
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}
