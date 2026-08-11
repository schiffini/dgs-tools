using System.ComponentModel.DataAnnotations;

namespace DgsTool.Models;

public enum RequestStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Rejected = 3
}

public enum RequestPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}

public class InformationRequest
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string RequesterName { get; set; } = string.Empty;

    [StringLength(150)]
    public string? RequesterId { get; set; }

    [Required]
    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Response { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [StringLength(150)]
    public string? AssignedTo { get; set; }
}
