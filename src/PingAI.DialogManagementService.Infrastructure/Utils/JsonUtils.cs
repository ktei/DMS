using System;
using System.Text.Json;

namespace PingAI.DialogManagementService.Infrastructure.Utils
{
    internal static class JsonUtils
    {
        public static string Serialize(object o, Action<JsonSerializerOptions>? configureOptions = null)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            configureOptions?.Invoke(options);

            return JsonSerializer.Serialize(o, options);
        }
        
        public static T Deserialize<T>(string s, Action<JsonSerializerOptions>? configureOptions = null)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            configureOptions?.Invoke(options);

            return JsonSerializer.Deserialize<T>(s, options);
        }
    }
}