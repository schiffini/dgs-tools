using DgsTool.Data;
using DgsTool.Models;
using DgsTool.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;

namespace DgsTool.Tests;

public class PlayerEditServiceTests
{
    private static readonly InMemoryDatabaseRoot Root = new();

    private static Player MakePlayer(int id = 1) => new()
    {
        Id = id,
        Username = "player1",
        Name = "Juan Perez",
        Password = "OldPass1!",
        OnlinePassword = "OldOnline1!",
        LineTypeId = 1,
        ProfileId = 2,
        BookId = 3,
        Email = "old@mail.com",
        Status = "E",
        AgentId = 100,
        CreditLimit = 1000m,
        TempCredit = 0m,
        OnlineAccess = true,
        OnlineMaxWager = 200m,
        OnlineMinWager = 10m,
        MaxWager = 500m,
        MinWager = 5m,
        FreePlayAmount = 0m,
        SettledFigure = 0m,
        ProfileLimitsId = 1,
        LastModificationUserId = null
    };

    [Fact]
    public async Task UpdateAsync_OnlyChangedFieldsGetAuditRows()
    {
        await using var context = await SeedAsync(MakePlayer());
        var sut = new PlayerEditService(context, NullLogger<PlayerEditService>.Instance);

        var changes = PlayerEditModel.FromPlayer(MakePlayer());
        changes.Name = "Juan Perez Updated";
        changes.CreditLimit = 2000m;

        await sut.UpdateAsync(1, changes, "rgonzalez");

        var updated = await context.Players.AsNoTracking().SingleAsync(p => p.Id == 1);
        Assert.Equal("Juan Perez Updated", updated.Name);
        Assert.Equal(2000m, updated.CreditLimit);

        var auditRows = await context.AuditLogs.AsNoTracking().ToListAsync();
        Assert.Equal(2, auditRows.Count);
        Assert.Contains(auditRows, a => a.FieldName == nameof(Player.Name) && a.OldValue == "Juan Perez" && a.NewValue == "Juan Perez Updated");
        Assert.Contains(auditRows, a => a.FieldName == nameof(Player.CreditLimit) && a.OldValue == "1000.00" && a.NewValue == "2000.00");
        Assert.All(auditRows, a =>
        {
            Assert.Equal(nameof(Player), a.EntityName);
            Assert.Equal("1", a.EntityId);
            Assert.Equal("UPDATE", a.Action);
            Assert.Equal("rgonzalez", a.ChangedByLoginName);
        });
    }

    [Fact]
    public async Task UpdateAsync_PasswordChanged_MasksAuditValueButWritesRealPassword()
    {
        await using var context = await SeedAsync(MakePlayer());
        var sut = new PlayerEditService(context, NullLogger<PlayerEditService>.Instance);

        var changes = PlayerEditModel.FromPlayer(MakePlayer());
        changes.Password = "NewPass1!";

        await sut.UpdateAsync(1, changes, "rgonzalez");

        var updated = await context.Players.AsNoTracking().SingleAsync(p => p.Id == 1);
        Assert.Equal("NewPass1!", updated.Password);

        var auditRow = await context.AuditLogs.AsNoTracking().SingleAsync(a => a.FieldName == nameof(Player.Password));
        Assert.Equal(PlayerEditService.MaskedCredentialMarker, auditRow.OldValue);
        Assert.Equal(PlayerEditService.MaskedCredentialMarker, auditRow.NewValue);
    }

    [Fact]
    public async Task UpdateAsync_OnlinePasswordChanged_MasksAuditValueButWritesRealPassword()
    {
        await using var context = await SeedAsync(MakePlayer());
        var sut = new PlayerEditService(context, NullLogger<PlayerEditService>.Instance);

        var changes = PlayerEditModel.FromPlayer(MakePlayer());
        changes.OnlinePassword = "NewOnline1!";

        await sut.UpdateAsync(1, changes, "rgonzalez");

        var updated = await context.Players.AsNoTracking().SingleAsync(p => p.Id == 1);
        Assert.Equal("NewOnline1!", updated.OnlinePassword);

        var auditRow = await context.AuditLogs.AsNoTracking().SingleAsync(a => a.FieldName == nameof(Player.OnlinePassword));
        Assert.Equal(PlayerEditService.MaskedCredentialMarker, auditRow.OldValue);
        Assert.Equal(PlayerEditService.MaskedCredentialMarker, auditRow.NewValue);
    }

    [Fact]
    public async Task UpdateAsync_BlankPassword_MeansNoChange_DoesNotOverwriteOrAudit()
    {
        await using var context = await SeedAsync(MakePlayer());
        var sut = new PlayerEditService(context, NullLogger<PlayerEditService>.Instance);

        var changes = PlayerEditModel.FromPlayer(MakePlayer());
        changes.Password = null;
        changes.OnlinePassword = string.Empty;
        changes.Name = "Changed Name"; // so the save isn't a total no-op

        await sut.UpdateAsync(1, changes, "rgonzalez");

        var updated = await context.Players.AsNoTracking().SingleAsync(p => p.Id == 1);
        Assert.Equal("OldPass1!", updated.Password);
        Assert.Equal("OldOnline1!", updated.OnlinePassword);

        var auditRows = await context.AuditLogs.AsNoTracking().ToListAsync();
        Assert.DoesNotContain(auditRows, a => a.FieldName == nameof(Player.Password));
        Assert.DoesNotContain(auditRows, a => a.FieldName == nameof(Player.OnlinePassword));
    }

    [Fact]
    public async Task UpdateAsync_NoFieldsChanged_IsNoOp()
    {
        await using var context = await SeedAsync(MakePlayer());
        var sut = new PlayerEditService(context, NullLogger<PlayerEditService>.Instance);

        var changes = PlayerEditModel.FromPlayer(MakePlayer());

        await sut.UpdateAsync(1, changes, "rgonzalez");

        var auditRows = await context.AuditLogs.AsNoTracking().ToListAsync();
        Assert.Empty(auditRows);
    }

    [Fact]
    public async Task UpdateAsync_UnknownPlayer_Throws()
    {
        await using var context = await SeedAsync(MakePlayer());
        var sut = new PlayerEditService(context, NullLogger<PlayerEditService>.Instance);

        var changes = PlayerEditModel.FromPlayer(MakePlayer());

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateAsync(999, changes, "rgonzalez"));
    }

    private static async Task<PlayerWriteDbContext> SeedAsync(Player player)
    {
        var dbName = Guid.NewGuid().ToString();

        var seedOptions = new DbContextOptionsBuilder<SeedOnlyPlayerContext>()
            .UseInMemoryDatabase(dbName, Root)
            .Options;
        await using (var seedContext = new SeedOnlyPlayerContext(seedOptions))
        {
            seedContext.Players.Add(player);
            await seedContext.SaveChangesAsync();
        }

        var options = new DbContextOptionsBuilder<PlayerWriteDbContext>()
            .UseInMemoryDatabase(dbName, Root)
            .Options;
        return new PlayerWriteDbContext(options);
    }
}

/// <summary>
/// Test-only twin of PlayerWriteDbContext's Player mapping, used purely to seed the shared
/// named InMemory database with a starting row.
/// </summary>
internal class SeedOnlyPlayerContext : DbContext
{
    public DbSet<Player> Players => Set<Player>();

    public SeedOnlyPlayerContext(DbContextOptions<SeedOnlyPlayerContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Ignore(p => p.Agent);
        });
    }
}
