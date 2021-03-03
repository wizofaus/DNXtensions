#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNXtensions
{
    public static class Json
    {
        static public T Deserialize<T>(string json, T _) => Deserialize<T>(json, DefaultOptions, _);
        static public T Deserialize<T>(string json, JsonSerializerOptions options, T _) => Deserialize<T>(json, options);
        static public T Deserialize<T>(string json) => Deserialize<T>(json, DefaultOptions);
        static public T Deserialize<T>(string json, JsonSerializerOptions options) => JsonSerializer.Deserialize<T>(json, options) ?? throw new JsonException();
        static public object Deserialize(string json, Type type) => JsonSerializer.Deserialize(json, type, DefaultOptions) ?? throw new JsonException();
        static public string Serialize<T>(T value) => JsonSerializer.Serialize(value, DefaultOptions);

        static public T DeserializeInto<T>(this JsonElement json, T destination)
        {
            var map = typeof(T).GetProperties().Select(p => KeyValuePair.Create(p.Name.ToLower(), p))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var e in json.EnumerateObject())
                if (e.Value.ValueKind != JsonValueKind.Null && map.TryGetValue(e.Name.ToLower(), out PropertyInfo? property))
                    property.SetValue(destination, Deserialize(e.Value.GetRawText(), property.PropertyType));
            return destination;
        }

        static public string SerializeWithNulls<T>(T value) => JsonSerializer.Serialize(value, CreateOptions(writeNulls: true));

        static public JsonSerializerOptions DefaultOptions = CreateOptions();

        static internal JsonSerializerOptions CreateOptions(bool writeNulls = false) =>
             new(JsonSerializerDefaults.Web)
             {
                 Converters = { new BooleanConverter(), new JsonStringNullableEnumConverter(), new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                 PropertyNameCaseInsensitive = true,
                 DefaultIgnoreCondition = writeNulls ? JsonIgnoreCondition.Never : JsonIgnoreCondition.WhenWritingNull
             };

        internal class BooleanConverter : JsonConverter<bool>
        {
            public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                reader.TokenType switch
                {
                    JsonTokenType.True => true,
                    JsonTokenType.False => false,
                    JsonTokenType.Number => reader.GetDecimal() != 0,
                    _ => bool.Parse((reader.GetString() ?? throw new JsonException()).ToLower())
                };
            public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
                writer.WriteBooleanValue(value);
        }

        public sealed class JsonStringNullableEnumConverter : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert) => Nullable.GetUnderlyingType(typeToConvert)?.IsEnum == true;

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
                (Activator.CreateInstance(typeof(EnumConverter<>).MakeGenericType(Nullable.GetUnderlyingType(typeToConvert)!)) as JsonConverter)!;
        }

        internal class EnumConverter<T> : JsonConverter<T?> where T: struct
        {
            public override bool CanConvert(Type type) => Nullable.GetUnderlyingType(type) == typeof(T);
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var enumString = reader.GetString();
                if (!Enum.TryParse<T>(enumString, out T value) && !Enum.TryParse<T>(enumString, ignoreCase: true, out value))
                {
                    System.Diagnostics.Trace.TraceError($"Couldn't convert {enumString} to {typeof(T)}");
                    return default;
                }
                return value;
            }
            public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options) =>
                writer.WriteStringValue(JsonNamingPolicy.CamelCase.ConvertName(value.ToString()!));
        }
    }
}
