namespace Logdash.Models;

public sealed class LogLevel
{
    private string Value { get; }
    public LogLevel(string value) => Value = value;
    
    public override string ToString() => Value;
    
    public static readonly LogLevel Error = new("error");
    public static readonly LogLevel Warn = new("warn");
    public static readonly LogLevel Info = new("info");
    public static readonly LogLevel Http = new("http");
    public static readonly LogLevel Verbose = new("verbose");
    public static readonly LogLevel Debug = new("debug");
    public static readonly LogLevel Silly = new("silly");
}