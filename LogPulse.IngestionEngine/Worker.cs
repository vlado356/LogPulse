using LogPulse.Core;
using LogPulse.Core.Data;
using LogPulse.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogPulse.IngestionEngine;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Random _random;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _random = new Random();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string[] levels = { "INFO", "WARNING", "ERROR", "CRITICAL" };
        string[] messages =
        {
            "User login successful",
            "Database connection timeout, retrying...",
            "Failed to process payment for user 42",
            "CPU usage above 95% on instance backend-01"
        };

        _logger.LogInformation("LogPulse Ingestion Engine started working...");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();

                var randomLevel = levels[_random.Next(levels.Length)];
                var randomMessage = messages[_random.Next(messages.Length)];

                var logEntry = new LogEntry
                {
                    LogLevel = randomLevel,
                    Message = randomMessage,
                    Timestamp = DateTime.UtcNow,
                    Source = "IngestionEngine",
                    MachineName = Environment.MachineName
                };

                dbContext.Logs.Add(logEntry);
                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Saved to DB: [{Level}] {Message}", logEntry.LogLevel, logEntry.Message);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}