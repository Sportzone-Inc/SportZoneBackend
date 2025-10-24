using System.ComponentModel.DataAnnotations;
using SportZone.Models;

namespace SportZone.DTOs;

public class CreatePostDto
{
    public string? ActivityId { get; set; }
    public string? Title { get; set; }
    
    [Required]
    public string Body { get; set; } = string.Empty;
    
    public PostType PostType { get; set; } = PostType.Text;
    public List<string>? MediaUrls { get; set; }
    public string? ThumbnailUrl { get; set; }
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public List<string>? Tags { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class UpdatePostDto
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public List<string>? MediaUrls { get; set; }
    public string? ThumbnailUrl { get; set; }
    public PostVisibility? Visibility { get; set; }
    public List<string>? Tags { get; set; }
    public bool? IsPinned { get; set; }
}

public class PostResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? ActivityId { get; set; }
    public string? Title { get; set; }
    public string Body { get; set; } = string.Empty;
    public PostType PostType { get; set; }
    public List<string> MediaUrls { get; set; } = new();
    public string? ThumbnailUrl { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public PostVisibility Visibility { get; set; }
    public bool IsPinned { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
