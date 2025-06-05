using System.Text.Json.Serialization;
using Logdash.Converters;

namespace Logdash.Models;

[JsonConverter(typeof(LogLevelConverter))]
internal sealed class LogLevel(string value)
{
    private string Value { get; } = value;

    public override string ToString() => Value;
    
    public static readonly LogLevel Error = new("error");
    public static readonly LogLevel Warn = new("warn");
    public static readonly LogLevel Info = new("info");
    public static readonly LogLevel Http = new("http");
    public static readonly LogLevel Verbose = new("verbose");
    public static readonly LogLevel Debug = new("debug");
    public static readonly LogLevel Silly = new("silly");
}