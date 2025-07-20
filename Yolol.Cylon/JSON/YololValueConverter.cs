using System;
using Newtonsoft.Json;
using Yolol.Execution;
using Type = System.Type;

namespace Yolol.Cylon.JSON
{
    public class YololValueConverter
        : JsonConverter<Value>
    {
        public override void WriteJson(JsonWriter writer, Value value, JsonSerializer serializer)
        {
            if (value.Type == Execution.Type.String)
                writer.WriteValue(value.String.ToString());
            else
                writer.WriteRawValue(value.Number.ToString());
        }

        public override Value ReadJson(JsonReader reader, Type objectType, Value existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType switch {
                JsonToken.String => new Value((string)reader.Value!),
                JsonToken.Integer => new Value((Number)Convert.ToDecimal(reader.Value!)),
                JsonToken.Float => new Value((Number)Convert.ToDecimal(reader.Value!)),
                _ => throw new InvalidOperationException($"Unexpected token type `{reader.TokenType}` for Yolol value")
            };
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;
    }
}
