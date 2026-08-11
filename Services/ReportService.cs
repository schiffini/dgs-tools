using DgsTool.Data;
using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

public class ReportService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ReportService> _logger;

    public ReportService(ApplicationDbContext db, ILogger<ReportService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<List<ReportDefinition>> GetAllAsync() =>
        _db.ReportDefinitions.AsNoTracking().OrderBy(r => r.Category).ThenBy(r => r.Name).ToListAsync();

    public async Task<ReportDefinition?> GetByIdAsync(int id) =>
        await _db.ReportDefinitions.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<ReportExecution>> GetExecutionsAsync(int reportId) =>
        await _db.ReportExecutions.AsNoTracking()
            .Where(e => e.ReportDefinitionId == reportId)
            .OrderByDescending(e => e.StartedAt)
            .Take(25)
            .ToListAsync();

    public async Task<ReportExecution> RunAsync(int reportId, string? executedBy)
    {
        var report = await _db.ReportDefinitions.FirstOrDefaultAsync(r => r.Id == reportId)
            ?? throw new InvalidOperationException($"Reporte {reportId} no encontrado.");

        var execution = new ReportExecution
        {
            ReportDefinitionId = report.Id,
            Status = ExecutionStatus.Running,
            StartedAt = DateTime.UtcNow,
            ExecutedBy = executedBy
        };

        _db.ReportExecutions.Add(execution);
        await _db.SaveChangesAsync();

        try
        {
            await Task.Delay(1500);
            execution.Status = ExecutionStatus.Completed;
            execution.FinishedAt = DateTime.UtcNow;
            execution.ResultFile = $"Reporte_{report.Code}_{execution.Id}.csv";
            execution.Notes = $"Se generó el reporte '{report.Name}' con éxito.";
            await _db.SaveChangesAsync();
            _logger.LogInformation("Reporte {Code} ejecutado por {User}", report.Code, executedBy);
            return execution;
        }
        catch (Exception ex)
        {
            execution.Status = ExecutionStatus.Failed;
            execution.FinishedAt = DateTime.UtcNow;
            execution.Notes = $"Error: {ex.Message}";
            await _db.SaveChangesAsync();
            throw;
        }
    }

    public async Task CreateAsync(ReportDefinition report, string? createdBy)
    {
        report.CreatedAt = DateTime.UtcNow;
        report.CreatedBy = createdBy;
        _db.ReportDefinitions.Add(report);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var report = await _db.ReportDefinitions.FirstOrDefaultAsync(r => r.Id == id);
        if (report is null) return;

        var executions = _db.ReportExecutions.Where(e => e.ReportDefinitionId == id);
        _db.ReportExecutions.RemoveRange(executions);
        _db.ReportDefinitions.Remove(report);
        await _db.SaveChangesAsync();
    }
}
