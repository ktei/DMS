namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class ResolutionDto
    {
        public ResolutionPartDto[]? Parts { get; set; }
        public FormResolutionDto? Form { get; set; }
        public string Type { get; set; }

        public ResolutionDto(ResolutionPartDto[]? parts, FormResolutionDto? form, string type)
        {
            Parts = parts;
            Form = form;
            Type = type;
        }

        public ResolutionDto()
        {
            
        }
    }
}