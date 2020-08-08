namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public static class ErrorDescriptions
    {
        public const string ProjectReadDenied = "You have no read access to current project";
        public const string ProjectWriteDenied = "You have no write access to current project";
        public const string IntentNotFound = "Intent does not exist";
        public const string EntityTypeNotFound = "EntityType does not exist";
        public const string EntityNameNotFound = "EntityName {0} does not exist";
    }
}