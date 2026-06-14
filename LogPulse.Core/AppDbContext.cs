using Microsoft.EntityFrameworkCore;

namespace LogPulse.Core;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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