namespace DgsTool.Models;

/// <summary>
/// One row per changed field. Lives in the external DGSDataTest database
/// (dbo.DgsToolAuditLog) alongside the data it audits - see
/// scripts/create-dgstool-auditlog.sql for the physical schema.
/// </summary>
public class PlayerAuditLog
{
    public long Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ChangedByLoginName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}
