using DgsTool.Data;
using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

public class AgentService
{
    private readonly ReportingDbContext _db;

    public AgentService(ReportingDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Agent> Items, int TotalCount)> GetPagedAsync(string? search, int page, int pageSize)
    {
        IQueryable<Agent> query = _db.Agents.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a =>
                a.Code.Contains(search) ||
                (a.Name != null && a.Name.Contains(search)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(a => a.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<AgentHierarchyNode>> GetHierarchyAsync()
    {
        var agents = await _db.Agents.AsNoTracking()
            .OrderBy(a => a.Code)
            .Select(a => new AgentHierarchySummary
            {
                Id = a.Id,
                Code = a.Code,
                Name = a.Name,
                Enable = a.Enable,
                Distributor = a.Distributor
            })
            .ToListAsync();

        var validIds = agents.Select(a => a.Id).ToHashSet();
        var childrenByParent = agents.ToLookup(a => a.Distributor);
        var visited = new HashSet<int>();

        AgentHierarchyNode BuildNode(AgentHierarchySummary agent)
        {
            visited.Add(agent.Id);
            var node = new AgentHierarchyNode { Agent = agent };

            foreach (var child in childrenByParent[agent.Id])
            {
                if (visited.Contains(child.Id))
                {
                    continue; // guards against cyclic Distributor references in the source data
                }

                node.Children.Add(BuildNode(child));
            }

            return node;
        }

        // Roots: no Distributor, or a Distributor that doesn't resolve to a loaded agent (dangling reference)
        var roots = agents.Where(a => a.Distributor is null || !validIds.Contains(a.Distributor.Value));
        return roots.Select(BuildNode).ToList();
    }
}
