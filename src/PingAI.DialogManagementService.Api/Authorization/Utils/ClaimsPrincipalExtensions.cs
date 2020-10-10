using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PingAI.DialogManagementService.Api.Authorization.Utils
{
    internal static class ClaimsPrincipalExtensions
    {
        public static bool HasAdminScope(this ClaimsPrincipal claimsPrincipal) =>
            claimsPrincipal.HasScope(Scopes.Admin);

        public static bool IsClient(this ClaimsPrincipal claimsPrincipal, string clientId)
        {
            var parts = (claimsPrincipal.Identity.Name ?? string.Empty).Split("@",
                StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 && parts[0] == clientId;
        }

        public static bool HasScope(this ClaimsPrincipal claimsPrincipal, params string[] scopes)
        {
            return claimsPrincipal.HasClaim(c =>
            {
                if (c.Type == "scope")
                {
                    var grantedScopes = new HashSet<string>(
                        c.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                    var wantedScopes = new HashSet<string>(scopes);
                    return wantedScopes.Any() && wantedScopes.IsSubsetOf(grantedScopes);
                }

                return false;
            });
        }
    }
}