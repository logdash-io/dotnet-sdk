using Logdash.Models;

namespace Logdash.Requests;

public record LogRequest(string Message, LogLevel Level, string CreatedAt);
