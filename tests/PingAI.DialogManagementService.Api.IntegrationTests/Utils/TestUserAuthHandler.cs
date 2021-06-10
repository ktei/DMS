using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    public class TestUserAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly TestUserFinder _testUserFinder;

        public TestUserAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, TestUserFinder testUserFinder) : base(options, logger, encoder,
            clock)
        {
            _testUserFinder = testUserFinder;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var testUser = await _testUserFinder.FindUser();
            var claims = new[] {new Claim(ClaimTypes.Name, testUser.Auth0Id)};
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return result;
        }
    }
}
