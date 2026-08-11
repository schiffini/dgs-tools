using DgsTool.Data;
using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

public class InformationRequestService
{
    private readonly ApplicationDbContext _db;

    public InformationRequestService(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<InformationRequest>> GetAllAsync() =>
        _db.InformationRequests.AsNoTracking()
            .OrderByDescending(r => r.Status == RequestStatus.Pending)
            .ThenByDescending(r => r.Priority)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task CreateAsync(InformationRequest request)
    {
        request.CreatedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;
        _db.InformationRequests.Add(request);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int id, RequestStatus status, string? response, string? assignedTo)
    {
        var request = await _db.InformationRequests.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new InvalidOperationException($"Solicitud {id} no encontrada.");

        request.Status = status;
        request.Response = response;
        request.AssignedTo = assignedTo;
        request.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
