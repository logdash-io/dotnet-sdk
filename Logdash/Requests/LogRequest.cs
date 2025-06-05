using Logdash.Models;

namespace Logdash.Requests;

internal record LogRequest(string Message, LogLevel Level, string CreatedAt, int SequenceNumber);
