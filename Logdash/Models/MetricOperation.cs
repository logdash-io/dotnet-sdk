using System.Text.Json.Serialization;
using Logdash.Converters;

namespace Logdash.Models;

[JsonConverter(typeof(MetricOperationConverter))]
public sealed class MetricOperation(string value)
{
    private string Value { get; } = value;

    public override string ToString() => Value;
    
    public static readonly MetricOperation Set = new("set");
    public static readonly MetricOperation Change = new("change");
}