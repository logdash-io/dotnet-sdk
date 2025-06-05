using System.Text.Json;
using System.Text.Json.Serialization;
using Logdash.Models;

namespace Logdash.Converters;

internal class LogLevelConverter : JsonConverter<LogLevel>
{
    public override LogLevel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!); // Or match against known instances

    public override void Write(Utf8JsonWriter writer, LogLevel value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}