using System.ComponentModel.DataAnnotations;

namespace DgsTool.Models;

public class ValueHistory
{
    public int Id { get; set; }

    public int ParameterValueId { get; set; }
    public ParameterValue? ParameterValue { get; set; }

    [Required]
    [StringLength(250)]
    public string OldValue { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string NewValue { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    [StringLength(150)]
    public string? ChangedBy { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }
}
