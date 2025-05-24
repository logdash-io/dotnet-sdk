using Logdash.Models;

namespace Logdash.Requests;

public record LogRequest(string Message, LogLevel LogLevel, string CreatedAt);
