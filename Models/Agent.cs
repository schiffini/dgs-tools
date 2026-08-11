using System.ComponentModel.DataAnnotations.Schema;

namespace DgsTool.Models;

public class Agent
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? OnlinePassword { get; set; }
    public short? BookId { get; set; }
    public int? Distributor { get; set; }
    public bool IsDistributor { get; set; }
    public bool? IsDistributed { get; set; }

    public short IdCurrency { get; set; }
    public byte IdAgentType { get; set; }
    public bool Enable { get; set; }
    public bool OnlineAccess { get; set; }
    public DateTime LastModification { get; set; }
    public short LastModificationUser { get; set; }
    public bool MasterAgentEnabled { get; set; }
    public string? Email { get; set; }

    [NotMapped]
    public bool IsExpanded { get; set; }
}
