namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateQueryDto
    {
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public int DisplayOrder { get; set; }
        public CreateIntentDto Intent { get; set; }
        public CreateResponseDto[] Responses { get; set; }
    }
}