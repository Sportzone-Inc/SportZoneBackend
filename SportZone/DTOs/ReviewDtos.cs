using System.ComponentModel.DataAnnotations;
using SportZone.Models;

namespace SportZone.DTOs;

public class CreateReviewDto
{
    [Required]
    public ReviewType ReviewType { get; set; }
    
    public string? ActivityId { get; set; }
    public string? RevieweeId { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    
    public string? Title { get; set; }
    public string? Comment { get; set; }
}

public class UpdateReviewDto
{
    [Range(1, 5)]
    public int? Rating { get; set; }
    
    public string? Title { get; set; }
    public string? Comment { get; set; }
}

public class AddReviewResponseDto
{
    [Required]
    public string Response { get; set; } = string.Empty;
}

public class ReviewResponseDto
{
    public string Id { get; set; } = string.Empty;
    public ReviewType ReviewType { get; set; }
    public string? ActivityId { get; set; }
    public string ReviewerId { get; set; } = string.Empty;
    public string? RevieweeId { get; set; }
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public int HelpfulCount { get; set; }
    public bool IsVerified { get; set; }
    public string? Response { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
