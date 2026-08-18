using DgsTool.Data;
using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

public class PlayerService
{
    private readonly ReportingDbContext _db;

    public PlayerService(ReportingDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Player> Items, int TotalCount)> GetPagedAsync(string? search, int page, int pageSize)
    {
        IQueryable<Player> query = _db.Players.AsNoTracking().Include(p => p.Agent);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Username.Contains(search) ||
                (p.Name != null && p.Name.Contains(search)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Dictionary<int, decimal>> GetBalancesAsync(IReadOnlyCollection<int> playerIds, CancellationToken cancellationToken = default)
    {
        if (playerIds.Count == 0)
        {
            return new Dictionary<int, decimal>();
        }

        return await _db.PlayerBalances.AsNoTracking()
            .Where(b => playerIds.Contains(b.IdPlayer))
            .ToDictionaryAsync(b => b.IdPlayer, b => b.CurrentBalance, cancellationToken);
    }
}
