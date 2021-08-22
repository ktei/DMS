namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateResponseFormDto
    {
        public Field[] Fields { get; set; }
        
        public class Field
        {
            public string Name { get; set; }
            public string DisplayName { get; set; } 
        }
    }
}