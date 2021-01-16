namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UploadResponse
    {
        public string UploadUrl { get; set; }

        public UploadResponse(string uploadUrl)
        {
            UploadUrl = uploadUrl;
        }

        public UploadResponse()
        {
            
        }
    }
}