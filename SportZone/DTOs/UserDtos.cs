using System.ComponentModel.DataAnnotations;

namespace SportZone.DTOs;

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? PreferredSport { get; set; }
}

public class UpdateUserDto
{
    [EmailAddress]
    public string? Email { get; set; }

    [MinLength(6)]
    public string? Password { get; set; }

    public string? Name { get; set; }

    public string? PreferredSport { get; set; }
}

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class UserResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? PreferredSport { get; set; }
    public DateTime CreatedAt { get; set; }
}
