using System.Text.Json.Serialization;
using Logdash.Models;

namespace Logdash.Requests;

public record MetricRequest(string Name, object Value, MetricOperation Operation,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? App);