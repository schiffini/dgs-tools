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
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

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

            entity.HasOne(p => p.Agent)
                .WithMany()
                .HasForeignKey(p => p.AgentId)
                .HasPrincipalKey(a => a.Id);
        });

        builder.Entity<Agent>(entity =>
        {
            entity.ToTable("AGENT", t => t.ExcludeFromMigrations());
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).HasColumnName("IdAgent");
            entity.Property(a => a.Name).HasColumnName("Name");
            entity.Property(a => a.Code).HasColumnName("Agent");
            entity.Property(a => a.Password).HasColumnName("Password");
            entity.Property(a => a.OnlinePassword).HasColumnName("OnlinePassword");
            entity.Property(a => a.BookId).HasColumnName("IdBook");
            entity.Property(a => a.Distributor).HasColumnName("Distributor");
            entity.Property(a => a.IsDistributor).HasColumnName("IsDistributor");
            entity.Property(a => a.IsDistributed).HasColumnName("IsDistributed");
            entity.Property(a => a.IdCurrency).HasColumnName("IdCurrency");
            entity.Property(a => a.IdAgentType).HasColumnName("IdAgentType");
            entity.Property(a => a.Enable).HasColumnName("Enable");
            entity.Property(a => a.OnlineAccess).HasColumnName("OnlineAccess");
            entity.Property(a => a.LastModification).HasColumnName("LastModification");
            entity.Property(a => a.LastModificationUser).HasColumnName("LastModificationUser");
            entity.Property(a => a.MasterAgentEnabled).HasColumnName("MasterAgentEnabled");
            entity.Property(a => a.Email).HasColumnName("Email");
        });

        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable("USERS", t => t.ExcludeFromMigrations());
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("IdUser");
            entity.Property(u => u.LoginName).HasColumnName("LoginName");
            entity.Property(u => u.Password).HasColumnName("Password");
            entity.Property(u => u.Name).HasColumnName("Name");
            entity.Property(u => u.Status).HasColumnName("Status");
            entity.Property(u => u.Type).HasColumnName("Type");
            entity.Property(u => u.IdUserProfile).HasColumnName("IdUserProfile");
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        throw new InvalidOperationException("ReportingDbContext is read-only; writes to the DGS reporting database are not permitted.");

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("ReportingDbContext is read-only; writes to the DGS reporting database are not permitted.");
}
