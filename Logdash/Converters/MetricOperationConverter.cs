using System.Text.Json;
using System.Text.Json.Serialization;
using Logdash.Models;

namespace Logdash.Converters;

public class MetricOperationConverter : JsonConverter<MetricOperation>
{
    public override MetricOperation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!); // Or match against known instances

    public override void Write(Utf8JsonWriter writer, MetricOperation value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}