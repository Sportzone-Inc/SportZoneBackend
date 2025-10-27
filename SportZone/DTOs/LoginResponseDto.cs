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
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c</example>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gebruikersnaam of email
        /// </summary>
        /// <example>john_doe</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User ID
        /// </summary>
        /// <example>507f1f77bcf86cd799439011</example>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Expiratie datum van de token
        /// </summary>
        /// <example>2025-10-27T20:34:21Z</example>
        public DateTime ExpiresAt { get; set; }
    }
}
