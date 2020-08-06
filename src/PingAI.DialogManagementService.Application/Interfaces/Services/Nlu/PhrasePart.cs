using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Nlu
{
    public class PhrasePart
    {
        public string Text { get; set; }
        public string? EntityName { get; set; }
        public string? EntityType { get; set; }
        public string? EntityValue { get; set; }
        public string Type { get; set; }
    }
}