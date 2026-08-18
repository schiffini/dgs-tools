namespace DgsTool.Models;

/// <summary>
/// One structural field (Agent/Profile/Book/LineType) changed in the Player edit dialog,
/// formatted for display in the structural-change-confirm-dialog.
/// </summary>
public record StructuralFieldChange(string FieldLabel, string OldValue, string NewValue);
