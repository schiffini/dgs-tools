using System.ComponentModel.DataAnnotations;

namespace DgsTool.Models;

public class ReportDefinition
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(80)]
    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(150)]
    public string? CreatedBy { get; set; }

    public ICollection<ReportExecution> Executions { get; set; } = new List<ReportExecution>();
}
