using DgsTool.Data;
using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

/// <summary>
/// Validates staff login credentials read-only against dbo.USERS in the external
/// reporting database. Deliberately does NOT use the legacy UserLogin/UserLogout
/// stored procedures (those write to dbo.UserLogons for simultaneous-login blocking
/// and compute unrelated legacy permission bits).
/// </summary>
public class ExternalUserService
{
    private readonly ReportingDbContext _db;

    public ExternalUserService(ReportingDbContext db)
    {
        _db = db;
    }

    public async Task<AppUser?> ValidateAsync(string loginName, string password)
    {
        if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var normalizedLoginName = loginName.ToUpper();

        var candidate = await _db.AppUsers.AsNoTracking()
            .Where(u => u.Type == 0 && u.Status)
            .Where(u => u.LoginName.ToUpper() == normalizedLoginName)
            .FirstOrDefaultAsync();

        if (candidate is null)
        {
            return null;
        }

        return string.Equals(candidate.Password, password, StringComparison.OrdinalIgnoreCase)
            ? candidate
            : null;
    }
}
