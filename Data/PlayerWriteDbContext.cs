using DgsTool.Models;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Data;

/// <summary>
/// The ONE write-capable path into the external DGSDataTest database. Deliberately separate
/// from <see cref="ReportingDbContext"/>: that context's read-only guarantee (SaveChanges
/// throws) is load-bearing for every other read surface in the app (Players, Agents,
/// Hierarchy, the balance badge, login) and must never be relaxed. This context maps only
/// the two entities this feature needs to write - Player, and the new PlayerAuditLog table -
/// over the same physical ReportingConnection connection string.
///
/// No SaveChanges override here: this context is meant to write.
/// </summary>
public class PlayerWriteDbContext : DbContext
{
    public PlayerWriteDbContext(DbContextOptions<PlayerWriteDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerAuditLog> AuditLogs => Set<PlayerAuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Column mapping copied from ReportingDbContext's Player entity block - do not alter
        // the original, this is an independent copy for the write path.
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

            // This context maps only Player + PlayerAuditLog (no Agent DbSet here) - ignore
            // the navigation so EF doesn't try to discover Agent as a related entity type.
            entity.Ignore(p => p.Agent);
        });

        builder.Entity<PlayerAuditLog>(entity =>
        {
            entity.ToTable("DgsToolAuditLog", t => t.ExcludeFromMigrations());
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).HasColumnName("IdAuditLog");
            entity.Property(a => a.EntityName).HasColumnName("EntityName");
            entity.Property(a => a.EntityId).HasColumnName("EntityId");
            entity.Property(a => a.FieldName).HasColumnName("FieldName");
            entity.Property(a => a.OldValue).HasColumnName("OldValue");
            entity.Property(a => a.NewValue).HasColumnName("NewValue");
            entity.Property(a => a.Action).HasColumnName("Action");
            entity.Property(a => a.ChangedByLoginName).HasColumnName("ChangedByLoginName");
            entity.Property(a => a.ChangedAt).HasColumnName("ChangedAt");
        });
    }
}
