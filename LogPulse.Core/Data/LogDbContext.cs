using LogPulse.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LogPulse.Core.Data;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
    {
    }

    public DbSet<LogEntry> Logs => Set<LogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<LogEntry>()
            .Property(l => l.Id)
            .HasDefaultValueSql("gen_random_uuid()");
    }
}