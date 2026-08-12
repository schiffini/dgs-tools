using DgsTool.Data;
using DgsTool.Models;
using DgsTool.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DgsTool.Tests;

public class ExternalUserServiceTests
{
    private static readonly InMemoryDatabaseRoot Root = new();

    [Fact]
    public async Task ValidateAsync_ValidCredentials_ReturnsUser()
    {
        await using var context = await SeedAsync(new AppUser
        {
            Id = 1, LoginName = "jdoe", Password = "Secret1", Status = true, Type = 0
        });
        var sut = new ExternalUserService(context);

        var result = await sut.ValidateAsync("jdoe", "Secret1");

        Assert.NotNull(result);
        Assert.Equal("jdoe", result!.LoginName);
    }

    [Fact]
    public async Task ValidateAsync_IsCaseInsensitiveForLoginNameAndPassword()
    {
        await using var context = await SeedAsync(new AppUser
        {
            Id = 1, LoginName = "jdoe", Password = "Secret1", Status = true, Type = 0
        });
        var sut = new ExternalUserService(context);

        var result = await sut.ValidateAsync("JDOE", "secret1");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ValidateAsync_WrongPassword_ReturnsNull()
    {
        await using var context = await SeedAsync(new AppUser
        {
            Id = 1, LoginName = "jdoe", Password = "Secret1", Status = true, Type = 0
        });
        var sut = new ExternalUserService(context);

        var result = await sut.ValidateAsync("jdoe", "WrongPassword");

        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateAsync_InactiveUser_ReturnsNull()
    {
        await using var context = await SeedAsync(new AppUser
        {
            Id = 1, LoginName = "jdoe", Password = "Secret1", Status = false, Type = 0
        });
        var sut = new ExternalUserService(context);

        var result = await sut.ValidateAsync("jdoe", "Secret1");

        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateAsync_NonStaffType_ReturnsNull()
    {
        await using var context = await SeedAsync(new AppUser
        {
            Id = 1, LoginName = "apiuser", Password = "Secret1", Status = true, Type = 3
        });
        var sut = new ExternalUserService(context);

        var result = await sut.ValidateAsync("apiuser", "Secret1");

        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateAsync_UnknownLogin_ReturnsNull()
    {
        await using var context = await SeedAsync(new AppUser
        {
            Id = 1, LoginName = "jdoe", Password = "Secret1", Status = true, Type = 0
        });
        var sut = new ExternalUserService(context);

        var result = await sut.ValidateAsync("nobody", "Secret1");

        Assert.Null(result);
    }

    [Theory]
    [InlineData(null, "Secret1")]
    [InlineData("", "Secret1")]
    [InlineData("jdoe", null)]
    [InlineData("jdoe", "")]
    public async Task ValidateAsync_NullOrEmptyLoginNameOrPassword_ReturnsNullWithoutThrowing(string? loginName, string? password)
    {
        await using var context = await SeedAsync(new AppUser
        {
            Id = 1, LoginName = "jdoe", Password = "Secret1", Status = true, Type = 0
        });
        var sut = new ExternalUserService(context);

        var result = await sut.ValidateAsync(loginName!, password!);

        Assert.Null(result);
    }

    private static async Task<ReportingDbContext> SeedAsync(params AppUser[] users)
    {
        var dbName = Guid.NewGuid().ToString();

        var seedOptions = new DbContextOptionsBuilder<SeedOnlyUsersContext>()
            .UseInMemoryDatabase(dbName, Root)
            .Options;
        await using (var seedContext = new SeedOnlyUsersContext(seedOptions))
        {
            seedContext.AppUsers.AddRange(users);
            await seedContext.SaveChangesAsync();
        }

        var options = new DbContextOptionsBuilder<ReportingDbContext>()
            .UseInMemoryDatabase(dbName, Root)
            .Options;
        return new ReportingDbContext(options);
    }
}

/// <summary>
/// Test-only twin of ReportingDbContext's AppUser mapping, used purely to seed the
/// shared named InMemory database — ReportingDbContext itself refuses all writes by design.
/// </summary>
internal class SeedOnlyUsersContext : DbContext
{
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    public SeedOnlyUsersContext(DbContextOptions<SeedOnlyUsersContext> options) : base(options)
    {
    }
}
