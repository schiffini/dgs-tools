using DgsTool.Data;
using DgsTool.Models;
using DgsTool.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DgsTool.Tests;

public class PlayerServiceTests
{
    private static readonly InMemoryDatabaseRoot Root = new();

    [Fact]
    public async Task GetBalancesAsync_EmptyInput_ReturnsEmptyDictionary()
    {
        await using var context = await SeedAsync(
            new PlayerBalance { IdPlayer = 1, CurrentBalance = 100m });
        var sut = new PlayerService(context);

        var result = await sut.GetBalancesAsync(Array.Empty<int>());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBalancesAsync_BatchedLookup_ReturnsOnlyRequestedPlayers()
    {
        await using var context = await SeedAsync(
            new PlayerBalance { IdPlayer = 1, CurrentBalance = 100.50m },
            new PlayerBalance { IdPlayer = 2, CurrentBalance = -25m },
            new PlayerBalance { IdPlayer = 3, CurrentBalance = 0m });
        var sut = new PlayerService(context);

        var result = await sut.GetBalancesAsync(new[] { 1, 2 });

        Assert.Equal(2, result.Count);
        Assert.Equal(100.50m, result[1]);
        Assert.Equal(-25m, result[2]);
        Assert.False(result.ContainsKey(3));
    }

    [Fact]
    public async Task GetBalancesAsync_MissingPlayerId_IsOmittedFromResultWithoutError()
    {
        await using var context = await SeedAsync(
            new PlayerBalance { IdPlayer = 1, CurrentBalance = 50m });
        var sut = new PlayerService(context);

        var result = await sut.GetBalancesAsync(new[] { 1, 999 });

        Assert.Single(result);
        Assert.True(result.ContainsKey(1));
        Assert.False(result.ContainsKey(999));
    }

    private static async Task<ReportingDbContext> SeedAsync(params PlayerBalance[] balances)
    {
        var dbName = Guid.NewGuid().ToString();

        var seedOptions = new DbContextOptionsBuilder<SeedOnlyPlayerBalancesContext>()
            .UseInMemoryDatabase(dbName, Root)
            .Options;
        await using (var seedContext = new SeedOnlyPlayerBalancesContext(seedOptions))
        {
            seedContext.PlayerBalances.AddRange(balances);
            await seedContext.SaveChangesAsync();
        }

        var options = new DbContextOptionsBuilder<ReportingDbContext>()
            .UseInMemoryDatabase(dbName, Root)
            .Options;
        return new ReportingDbContext(options);
    }
}

/// <summary>
/// Test-only twin of ReportingDbContext's PlayerBalance mapping, used purely to seed the
/// shared named InMemory database - ReportingDbContext itself refuses all writes by design.
/// </summary>
internal class SeedOnlyPlayerBalancesContext : DbContext
{
    public DbSet<PlayerBalance> PlayerBalances => Set<PlayerBalance>();

    public SeedOnlyPlayerBalancesContext(DbContextOptions<SeedOnlyPlayerBalancesContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<PlayerBalance>().HasKey(b => b.IdPlayer);
    }
}
