using System.ComponentModel.DataAnnotations;

namespace DgsTool.Models;

public enum ParameterDataType
{
    String = 0,
    Number = 1,
    Boolean = 2,
    Date = 3
}

public class ParameterValue
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Value { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(80)]
    public string Category { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Unit { get; set; }

    public ParameterDataType DataType { get; set; } = ParameterDataType.String;

    public bool IsActive { get; set; } = true;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(150)]
    public string? UpdatedBy { get; set; }

    public ICollection<ValueHistory> History { get; set; } = new List<ValueHistory>();
}
