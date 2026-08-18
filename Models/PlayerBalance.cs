namespace DgsTool.Models;

/// <summary>
/// Lean read-only projection of dbo.PLAYERSTATISTIC used solely to fetch a player's
/// current balance for the live balance badge on the Players page.
/// </summary>
public class PlayerBalance
{
    public int IdPlayer { get; set; }
    public decimal CurrentBalance { get; set; }
}
