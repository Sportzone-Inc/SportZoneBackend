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
        /// Gebruikersnaam of email
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Expiratie datum van de token
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}
