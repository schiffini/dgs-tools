using DgsTool.Data;
using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

public class PlayerEditService
{
    /// <summary>
    /// Literal marker written to OldValue/NewValue for Password/OnlinePassword audit rows -
    /// the real credential value is never written to the audit log.
    /// </summary>
    public const string MaskedCredentialMarker = "(cambiado)";

    private readonly PlayerWriteDbContext _db;
    private readonly ILogger<PlayerEditService> _logger;

    public PlayerEditService(PlayerWriteDbContext db, ILogger<PlayerEditService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Loads the tracked Player, diffs each editable field against <paramref name="changes"/>,
    /// sets only the fields that actually differ and adds one PlayerAuditLog row per changed
    /// field (masking Password/OnlinePassword), then writes everything in a single
    /// SaveChangesAsync call - mirrors ParameterValueService.UpdateValueAsync's
    /// update-row-plus-history-row-in-one-transaction shape.
    ///
    /// No-op (no DB write, no audit rows) if nothing actually changed.
    /// </summary>
    public async Task UpdateAsync(int playerId, PlayerEditModel changes, string changedByLoginName)
    {
        ArgumentNullException.ThrowIfNull(changes);

        var player = await _db.Players.FirstOrDefaultAsync(p => p.Id == playerId)
            ?? throw new InvalidOperationException($"Player {playerId} no encontrado.");

        var changedAt = DateTime.UtcNow;
        var anyChanges = false;

        void Audit(string field, string? oldValue, string? newValue)
        {
            _db.AuditLogs.Add(new PlayerAuditLog
            {
                EntityName = nameof(Player),
                EntityId = playerId.ToString(),
                FieldName = field,
                OldValue = oldValue,
                NewValue = newValue,
                Action = "UPDATE",
                ChangedByLoginName = changedByLoginName,
                ChangedAt = changedAt
            });
            anyChanges = true;
        }

        void Diff<T>(string field, T oldValue, T newValue, Action apply)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return;
            }

            Audit(field, FormatValue(oldValue), FormatValue(newValue));
            apply();
        }

        Diff(nameof(Player.Name), player.Name, changes.Name, () => player.Name = changes.Name);
        Diff(nameof(Player.Email), player.Email, changes.Email, () => player.Email = changes.Email);
        Diff(nameof(Player.Status), player.Status, changes.Status, () => player.Status = changes.Status);

        Diff(nameof(Player.CreditLimit), player.CreditLimit, changes.CreditLimit, () => player.CreditLimit = changes.CreditLimit);
        Diff(nameof(Player.MaxWager), player.MaxWager, changes.MaxWager, () => player.MaxWager = changes.MaxWager);
        Diff(nameof(Player.MinWager), player.MinWager, changes.MinWager, () => player.MinWager = changes.MinWager);
        Diff(nameof(Player.OnlineMaxWager), player.OnlineMaxWager, changes.OnlineMaxWager, () => player.OnlineMaxWager = changes.OnlineMaxWager);
        Diff(nameof(Player.OnlineMinWager), player.OnlineMinWager, changes.OnlineMinWager, () => player.OnlineMinWager = changes.OnlineMinWager);
        Diff(nameof(Player.TempCredit), player.TempCredit, changes.TempCredit, () => player.TempCredit = changes.TempCredit);
        Diff(nameof(Player.FreePlayAmount), player.FreePlayAmount, changes.FreePlayAmount, () => player.FreePlayAmount = changes.FreePlayAmount);

        // Structural fields - the UI is responsible for gating these behind the
        // structural-change-confirm-dialog BEFORE calling UpdateAsync at all; by the time we
        // get here, any structural change has already been confirmed.
        Diff(nameof(Player.LineTypeId), player.LineTypeId, changes.LineTypeId, () => player.LineTypeId = changes.LineTypeId);
        Diff(nameof(Player.ProfileId), player.ProfileId, changes.ProfileId, () => player.ProfileId = changes.ProfileId);
        Diff(nameof(Player.BookId), player.BookId, changes.BookId, () => player.BookId = changes.BookId);
        Diff(nameof(Player.AgentId), player.AgentId, changes.AgentId, () => player.AgentId = changes.AgentId);
        Diff(nameof(Player.OnlineAccess), player.OnlineAccess, changes.OnlineAccess, () => player.OnlineAccess = changes.OnlineAccess);

        // Password/OnlinePassword: blank means "don't change" - never overwrite with an empty
        // string. When a real new value is provided and differs from the current one, write
        // the real value to Player but mask both sides of the audit row.
        if (!string.IsNullOrEmpty(changes.Password) && !string.Equals(player.Password, changes.Password, StringComparison.Ordinal))
        {
            Audit(nameof(Player.Password), MaskedCredentialMarker, MaskedCredentialMarker);
            player.Password = changes.Password;
        }

        if (!string.IsNullOrEmpty(changes.OnlinePassword) && !string.Equals(player.OnlinePassword, changes.OnlinePassword, StringComparison.Ordinal))
        {
            Audit(nameof(Player.OnlinePassword), MaskedCredentialMarker, MaskedCredentialMarker);
            player.OnlinePassword = changes.OnlinePassword;
        }

        if (!anyChanges)
        {
            return;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Player {PlayerId} updated by {User}", playerId, changedByLoginName);
    }

    private static string? FormatValue<T>(T value) => value switch
    {
        null => null,
        decimal d => d.ToString("0.00"),
        bool b => b ? "1" : "0",
        _ => value.ToString()
    };
}
