using DgsTool.Data;
using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

public class DashboardService
{
    private readonly ApplicationDbContext _db;

    public DashboardService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> GetActiveParametersCountAsync() =>
        await _db.ParameterValues.CountAsync(p => p.IsActive);

    public async Task<int> GetActiveReportsCountAsync() =>
        await _db.ReportDefinitions.CountAsync(r => r.IsActive);

    public async Task<int> GetPendingRequestsCountAsync() =>
        await _db.InformationRequests.CountAsync(r => r.Status == RequestStatus.Pending);

    public async Task<int> GetCompletedExecutionsCountAsync() =>
        await _db.ReportExecutions.CountAsync(e => e.Status == ExecutionStatus.Completed);

    public async Task<List<(string Category, int Count)>> GetParametersByCategoryAsync() =>
        await _db.ParameterValues.AsNoTracking()
            .Where(p => p.IsActive)
            .GroupBy(p => p.Category)
            .Select(g => new ValueTuple<string, int>(g.Key, g.Count()))
            .OrderByDescending(g => g.Item2)
            .ToListAsync();

    public async Task<List<ReportExecution>> GetRecentExecutionsAsync() =>
        await _db.ReportExecutions.AsNoTracking()
            .Include(e => e.ReportDefinition)
            .OrderByDescending(e => e.StartedAt)
            .Take(8)
            .ToListAsync();

    public async Task<List<InformationRequest>> GetRecentRequestsAsync() =>
        await _db.InformationRequests.AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Take(6)
            .ToListAsync();

    public async Task<List<(RequestStatus Status, int Count)>> GetRequestsByStatusAsync() =>
        await _db.InformationRequests.AsNoTracking()
            .GroupBy(r => r.Status)
            .Select(g => new ValueTuple<RequestStatus, int>(g.Key, g.Count()))
            .ToListAsync();
}
