using System.ComponentModel.DataAnnotations.Schema;

namespace DgsTool.Models;

public class Player
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? OnlinePassword { get; set; }
    public short LineTypeId { get; set; }
    public short ProfileId { get; set; }
    public short? BookId { get; set; }
    public string? Email { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? AgentId { get; set; }
    public Agent? Agent { get; set; }

    public decimal CreditLimit { get; set; }
    public decimal TempCredit { get; set; }
    public bool OnlineAccess { get; set; }
    public decimal OnlineMaxWager { get; set; }
    public decimal OnlineMinWager { get; set; }
    public decimal MaxWager { get; set; }
    public decimal MinWager { get; set; }
    public decimal FreePlayAmount { get; set; }
    public decimal SettledFigure { get; set; }
    public short ProfileLimitsId { get; set; }
    public short? LastModificationUserId { get; set; }

    [NotMapped]
    public bool IsExpanded { get; set; }
}
