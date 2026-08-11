using DgsTool.Data;
using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

public class ParameterValueService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ParameterValueService> _logger;

    public ParameterValueService(ApplicationDbContext db, ILogger<ParameterValueService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<List<ParameterValue>> GetAllAsync() =>
        _db.ParameterValues.AsNoTracking().OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync();

    public Task<List<ParameterValue>> GetActiveByCategoryAsync(string category) =>
        _db.ParameterValues.AsNoTracking().Where(p => p.IsActive && p.Category == category).ToListAsync();

    public Task<List<string>> GetCategoriesAsync() =>
        _db.ParameterValues.AsNoTracking().Where(p => p.IsActive).Select(p => p.Category).Distinct().OrderBy(c => c).ToListAsync();

    public async Task<ParameterValue?> GetByIdAsync(int id) =>
        await _db.ParameterValues.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<ValueHistory>> GetHistoryAsync(int parameterId) =>
        await _db.ValueHistories.AsNoTracking()
            .Where(h => h.ParameterValueId == parameterId)
            .OrderByDescending(h => h.ChangedAt)
            .Take(50)
            .ToListAsync();

    public async Task UpdateValueAsync(int id, string newValue, string? reason, string? changedBy)
    {
        var parameter = await _db.ParameterValues.FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new InvalidOperationException($"Parámetro {id} no encontrado.");

        if (string.Equals(parameter.Value, newValue, StringComparison.Ordinal))
            return;

        _db.ValueHistories.Add(new ValueHistory
        {
            ParameterValueId = parameter.Id,
            OldValue = parameter.Value,
            NewValue = newValue,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = changedBy,
            Reason = reason
        });

        parameter.Value = newValue;
        parameter.UpdatedAt = DateTime.UtcNow;
        parameter.UpdatedBy = changedBy;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Parámetro {Code} actualizado de '{Old}' a '{New}' por {User}", parameter.Code, parameter.Value, newValue, changedBy);
    }

    public async Task CreateAsync(ParameterValue parameter, string? createdBy)
    {
        parameter.UpdatedAt = DateTime.UtcNow;
        parameter.UpdatedBy = createdBy;
        _db.ParameterValues.Add(parameter);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var parameter = await _db.ParameterValues.FirstOrDefaultAsync(p => p.Id == id);
        if (parameter is null) return;

        var histories = _db.ValueHistories.Where(h => h.ParameterValueId == id);
        _db.ValueHistories.RemoveRange(histories);
        _db.ParameterValues.Remove(parameter);
        await _db.SaveChangesAsync();
    }
}
