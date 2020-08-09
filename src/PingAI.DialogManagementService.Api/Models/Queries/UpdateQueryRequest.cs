using System;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class UpdateQueryRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public int DisplayOrder { get; set; }
        public CreateIntentDto Intent { get; set; }
        public CreateResponseDto Response { get; set; }
    }
}