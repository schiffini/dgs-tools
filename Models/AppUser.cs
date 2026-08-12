namespace DgsTool.Models;

/// <summary>
/// Lean read-only projection of the external dbo.USERS table (DGSDataTest), used to
/// validate staff login credentials. Not the legacy UserLogin/UserLogons flow.
/// </summary>
public class AppUser
{
    public int Id { get; set; }
    public string LoginName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool Status { get; set; }
    public byte Type { get; set; }
    public int? IdUserProfile { get; set; }
}
