using LogPulse.Core.Data;
using LogPulse.IngestionEngine;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<Worker>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

app.MapGet("/api/logs", async (
    LogDbContext db,
    int limit = 20,
    string? level = null,
    string? source = null,
    string? sortBy = "date",
    string? date = null) =>
{
    var query = db.Logs.AsQueryable();

    if (!string.IsNullOrEmpty(level))
    {
        query = query.Where(l => l.LogLevel.ToUpper() == level.ToUpper());
    }

    if (!string.IsNullOrEmpty(source))
    {
        query = query.Where(l => l.Source == source);
    }

    if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
    {
        DateTime startDate = DateTime.SpecifyKind(parsedDate.Date, DateTimeKind.Utc);
        DateTime endDate = startDate.AddDays(1);

        query = query.Where(l => l.Timestamp >= startDate && l.Timestamp < endDate);
    }

    query = sortBy.ToLower() switch
    {
        "level_asc" => query.OrderBy(l => l.LogLevel),
        "level_desc" => query.OrderByDescending(l => l.LogLevel),
        "source" => query.OrderBy(l => l.Source),
        _ => query.OrderByDescending(l => l.Timestamp)
    };

    if (limit <= 0) limit = 20;

    return await query.Take(limit).ToListAsync();
});

app.MapGet("/api/logs/sources", async (LogDbContext db) =>
{
    return await db.Logs
        .Select(l => l.Source)
        .Distinct() 
        .ToListAsync();
});

app.Run();