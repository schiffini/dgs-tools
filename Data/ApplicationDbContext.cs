using DgsTool.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ParameterValue> ParameterValues => Set<ParameterValue>();
    public DbSet<ValueHistory> ValueHistories => Set<ValueHistory>();
    public DbSet<ReportDefinition> ReportDefinitions => Set<ReportDefinition>();
    public DbSet<ReportExecution> ReportExecutions => Set<ReportExecution>();
    public DbSet<InformationRequest> InformationRequests => Set<InformationRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ParameterValue>()
            .HasIndex(p => p.Code)
            .IsUnique();

        builder.Entity<ReportDefinition>()
            .HasIndex(r => r.Code)
            .IsUnique();
    }
}
