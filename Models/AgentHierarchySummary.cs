namespace DgsTool.Models;

public class AgentHierarchySummary
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool Enable { get; set; }
    public int? Distributor { get; set; }
}
