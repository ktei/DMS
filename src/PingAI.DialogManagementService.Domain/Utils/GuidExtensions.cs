using System;

namespace PingAI.DialogManagementService.Domain.Utils
{
    public static class GuidExtensions
    {
        public static bool IsEmpty(this Guid target) => target == Guid.Empty;
    }
}
