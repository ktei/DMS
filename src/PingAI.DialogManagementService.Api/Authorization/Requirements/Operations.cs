using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace PingAI.DialogManagementService.Api.Authorization.Requirements
{
    public static class Operations
    {
        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement { Name = nameof(Read) };
        public static OperationAuthorizationRequirement Write =
            new OperationAuthorizationRequirement { Name = nameof(Write) };
    }
}