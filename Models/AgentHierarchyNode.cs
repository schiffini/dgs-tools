namespace DgsTool.Models;

public class AgentHierarchyNode
{
    public AgentHierarchySummary Agent { get; set; } = null!;
    public List<AgentHierarchyNode> Children { get; set; } = new();
}
