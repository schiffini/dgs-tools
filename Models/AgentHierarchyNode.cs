namespace DgsTool.Models;

public class AgentHierarchyNode
{
    public AgentHierarchySummary Agent { get; set; } = null!;
    public List<AgentHierarchyNode> Children { get; set; } = new();

    public int CountDescendants() => Children.Sum(c => 1 + c.CountDescendants());
}
