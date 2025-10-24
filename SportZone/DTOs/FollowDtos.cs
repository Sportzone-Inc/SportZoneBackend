using System.ComponentModel.DataAnnotations;

namespace SportZone.DTOs;

public class CreateFollowDto
{
    [Required]
    public string FollowingId { get; set; } = string.Empty;
}

public class FollowResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string FollowerId { get; set; } = string.Empty;
    public string FollowingId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class FollowStatsDto
{
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
}
