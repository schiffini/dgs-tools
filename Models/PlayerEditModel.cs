namespace DgsTool.Models;

/// <summary>
/// Editable snapshot of a Player used by the edit dialog / PlayerEditService.
/// Deliberately separate from <see cref="Player"/>: that type's Password/OnlinePassword
/// properties hold the real current value when read via ReportingDbContext, and reusing it
/// here risks accidentally binding/displaying the real password in the form.
///
/// Password/OnlinePassword are null/empty here to mean "don't change" - never overwrite the
/// real column with an empty string.
/// </summary>
public class PlayerEditModel
{
    // Perfil / Contacto
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string Status { get; set; } = string.Empty;

    // Financieros
    public decimal CreditLimit { get; set; }
    public decimal MaxWager { get; set; }
    public decimal MinWager { get; set; }
    public decimal OnlineMaxWager { get; set; }
    public decimal OnlineMinWager { get; set; }
    public decimal TempCredit { get; set; }
    public decimal FreePlayAmount { get; set; }

    // Credenciales - null/empty means "no change"
    public string? Password { get; set; }
    public string? OnlinePassword { get; set; }

    // Estructurales
    public short LineTypeId { get; set; }
    public short ProfileId { get; set; }
    public short? BookId { get; set; }
    public int? AgentId { get; set; }
    public bool OnlineAccess { get; set; }

    public static PlayerEditModel FromPlayer(Player player) => new()
    {
        Name = player.Name,
        Email = player.Email,
        Status = player.Status,
        CreditLimit = player.CreditLimit,
        MaxWager = player.MaxWager,
        MinWager = player.MinWager,
        OnlineMaxWager = player.OnlineMaxWager,
        OnlineMinWager = player.OnlineMinWager,
        TempCredit = player.TempCredit,
        FreePlayAmount = player.FreePlayAmount,
        Password = null,
        OnlinePassword = null,
        LineTypeId = player.LineTypeId,
        ProfileId = player.ProfileId,
        BookId = player.BookId,
        AgentId = player.AgentId,
        OnlineAccess = player.OnlineAccess
    };
}
