namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class IntegrationDto
    {
        public string[] Connections { get; set; }

        public IntegrationDto(string[] connections)
        {
            Connections = connections;
        }

        public IntegrationDto()
        {
            
        }
    }
}