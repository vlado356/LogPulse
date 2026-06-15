using LogPulse.Core.Data;
using LogPulse.IngestionEngine;
using LogPulse.Core.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.MapPost("/api/logs", async (LogEntry log, LogDbContext db) =>
{
    log.Timestamp = DateTime.UtcNow;

    db.Logs.Add(log);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Log accepted successfully!" });
});

app.Run();