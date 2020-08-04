using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    public static class HttpResponseMessageAssert
    {
        public static async Task IsOk(this HttpResponseMessage response)
        {
            var pass = response.StatusCode == HttpStatusCode.OK;
            if (!pass)
            {
                var message = await response.Content.ReadAsStringAsync();
                True(pass, $"Expected response to be OK, but got {response.StatusCode} with message {message}");
            }

            True(pass);
        }
    }
}