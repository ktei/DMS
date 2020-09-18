namespace PingAI.DialogManagementService.Domain.Model
{
    public static class Defaults
    {
        public const string WidgetTitle = "iiiknow";
        public const string WidgetColor = "#2196f3";
        public const string WidgetDescription = "iiiknow chatbot";
        public const string GreetingMessage = "Hello, how may I help you?";

        public static readonly string[] EnquiryEntityNames = new []
        {
            "NAME", "PHONE", "EMAIL", "JOB_TITLE", "ORGANISATION_NAME"
        };
    }
}
