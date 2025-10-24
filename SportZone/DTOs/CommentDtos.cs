using System.ComponentModel.DataAnnotations;

namespace SportZone.DTOs;

public class CreateCommentDto
{
    [Required]
    public string PostId { get; set; } = string.Empty;
    
    public string? ParentCommentId { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Body { get; set; } = string.Empty;
}

public class UpdateCommentDto
{
    [Required]
    [MinLength(1)]
    public string Body { get; set; } = string.Empty;
}

public class CommentResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? ParentCommentId { get; set; }
    public string Body { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
