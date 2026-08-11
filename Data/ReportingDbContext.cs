using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Data;

/// <summary>
/// Connects to the external DGS reporting database (DGSDataTest). Read-only by design:
/// SaveChanges is disabled here in code because the SQL login configured for this
/// connection is NOT restricted to SELECT at the server level.
/// </summary>
public class ReportingDbContext : DbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Player>(entity =>
        {
            entity.ToTable("Player", t => t.ExcludeFromMigrations());
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("IdPlayer");
            entity.Property(p => p.Username).HasColumnName("Player");
            entity.Property(p => p.Name).HasColumnName("Name");
            entity.Property(p => p.Password).HasColumnName("Password");
            entity.Property(p => p.OnlinePassword).HasColumnName("OnlinePassword");
            entity.Property(p => p.LineTypeId).HasColumnName("IdLineType");
            entity.Property(p => p.ProfileId).HasColumnName("IdProfile");
            entity.Property(p => p.BookId).HasColumnName("IdBook");
            entity.Property(p => p.Email).HasColumnName("Email");
            entity.Property(p => p.Status).HasColumnName("Status");
            entity.Property(p => p.AgentId).HasColumnName("IdAgent");
            entity.Property(p => p.CreditLimit).HasColumnName("CreditLimit");
            entity.Property(p => p.TempCredit).HasColumnName("TempCredit");
            entity.Property(p => p.OnlineAccess).HasColumnName("OnlineAccess");
            entity.Property(p => p.OnlineMaxWager).HasColumnName("OnlineMaxWager");
            entity.Property(p => p.OnlineMinWager).HasColumnName("OnlineMinWager");
            entity.Property(p => p.MaxWager).HasColumnName("MaxWager");
            entity.Property(p => p.MinWager).HasColumnName("MinWager");
            entity.Property(p => p.FreePlayAmount).HasColumnName("FreePlayAmount");
            entity.Property(p => p.SettledFigure).HasColumnName("SettledFigure");
            entity.Property(p => p.ProfileLimitsId).HasColumnName("IdProfileLimits");
            entity.Property(p => p.LastModificationUserId).HasColumnName("LastModificationUser");
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        throw new InvalidOperationException("ReportingDbContext is read-only; writes to the DGS reporting database are not permitted.");

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("ReportingDbContext is read-only; writes to the DGS reporting database are not permitted.");
}
