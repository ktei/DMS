using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public static class Defaults
    {
        public const string WidgetTitle = "iiiknow";
        public const string WidgetColor = "#2196f3";
        public const string WidgetDescription = "iiiknow chatbot";
        public const string GreetingMessage = "Hello, how may I help you?";
        public const string BusinessTimezone = "Australia/Sydney";
        public const string FallbackMessage = "Sorry, I'm still learning. Could you rephrase what you said just now?";
        public static readonly DateTime BusinessTimeStartUtc = DateTime.UtcNow.Date.AddHours(9); // 9 AM
        public static readonly DateTime BusinessTimeEndUtc = DateTime.UtcNow.Date.AddHours(17); // 5 PM

        public static readonly string[] EnquiryEntityNames = new []
        {
            "NAME", "PHONE", "EMAIL", "JOB_TITLE", "ORGANISATION_NAME"
        };
    }
}
