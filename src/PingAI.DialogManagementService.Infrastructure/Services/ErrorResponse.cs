using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Infrastructure.Services
{
    public class ErrorResponse
    {
        public string[] Errors { get; private set; }

        public static async Task<ErrorResponse> BuildFromHttpContent(HttpContent content)
        {
            try
            {
                var errorResponse = await content.ReadFromJsonAsync<ErrorResponse>();
                return errorResponse;
            }
            catch (Exception e)
            {
                var message = await content.ReadAsStringAsync();
                return new ErrorResponse
                {
                    Errors = new[] {message}
                };
            }
        }

        public override string ToString() => string.Join(Environment.NewLine, Errors ?? new string[0]);
    }
}