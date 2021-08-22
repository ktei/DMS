namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateConditionDto
    {
        public string Operator { get; set; }
        public string? Value { get; set; }
        public string? EntityNameId { get; set; }
        public string Comparer { get; set; } 
    }
}