using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Infrastructure.Utils
{
    public static class JsonUtils
    {
        public static JsonSerializerOptions DefaultSerializerOptions => new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreReadOnlyProperties = false,
            Converters =
            {
                new JsonStringEnumConverter(),
                new DateTimeConverter()
            }
        };

        public static void UpdateToDefaultOptions(JsonSerializerOptions options)
        {
            var defaultOptions = DefaultSerializerOptions;
            options.IgnoreNullValues = defaultOptions.IgnoreNullValues;
            options.PropertyNamingPolicy = defaultOptions.PropertyNamingPolicy;
            options.DictionaryKeyPolicy = defaultOptions.DictionaryKeyPolicy;
            options.IgnoreReadOnlyProperties = defaultOptions.IgnoreReadOnlyProperties;
            foreach (var converter in defaultOptions.Converters)
            {
                options.Converters.Add(converter);
            }
        }
        
        public static string Serialize(object o, Action<JsonSerializerOptions>? configureOptions = null)
        {
            var options = DefaultSerializerOptions;
            configureOptions?.Invoke(options);

            return JsonSerializer.Serialize(o, options);
        }

        public static T? TryDeserialize<T>(string s, Action<JsonSerializerOptions>? configureOptions = null)
            where T : class
        {
            try
            {
                return Deserialize<T>(s, configureOptions);
            }
            catch
            {
                return default;
            }
        }

        public static T Deserialize<T>(string s, Action<JsonSerializerOptions>? configureOptions = null) where T : class
        {
            var options = DefaultSerializerOptions;
            configureOptions?.Invoke(options);

            return JsonSerializer.Deserialize<T>(s, options);
        }
    }
    
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
        }
    }
}