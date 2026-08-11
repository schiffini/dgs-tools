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
}
