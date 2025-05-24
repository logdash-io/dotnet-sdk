namespace Logdash.Models;

public sealed class MetricOperation
{
    private string Value { get; }
    public MetricOperation(string value) => Value = value;
    
    public override string ToString() => Value;
    
    public static readonly MetricOperation Set = new("set");
    public static readonly MetricOperation Change = new("change");
}