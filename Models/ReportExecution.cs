using System.ComponentModel.DataAnnotations;

namespace DgsTool.Models;

public enum ExecutionStatus
{
    Pending = 0,
    Running = 1,
    Completed = 2,
    Failed = 3
}

public class ReportExecution
{
    public int Id { get; set; }

    public int ReportDefinitionId { get; set; }
    public ReportDefinition? ReportDefinition { get; set; }

    public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAt { get; set; }

    [StringLength(250)]
    public string? ResultFile { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(150)]
    public string? ExecutedBy { get; set; }
}
