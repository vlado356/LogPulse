using System;
using System.Collections.Generic;
using System.Text;

namespace LogPulse.Core.Models;

public class LogEntry
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public required string  LogLevel { get; set; }
    public required string Message { get; set; }
    public  required string Source { get; set; }
    public string? Exception { get; set; }
    public required string MachineName { get; set; }
}
