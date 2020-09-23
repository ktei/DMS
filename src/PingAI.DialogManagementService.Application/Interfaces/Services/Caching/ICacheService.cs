using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Caching
{
    public interface ICacheService
    {
        Task SetObject<T>(string key, T value) where T : class, new();

        Task<T?> GetObject<T>(string key, T? fallbackValue = default) where T : class, new();

        Task SetString(string key, string value);

        Task<string?> GetString(string key, string? fallbackValue = null);
    }
}