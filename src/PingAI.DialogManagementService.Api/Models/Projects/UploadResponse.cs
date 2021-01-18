namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UploadResponse
    {
        public string UploadUrl { get; set; }
        public string PublicUrl { get; set; }

        public UploadResponse(string uploadUrl, string publicUrl)
        {
            UploadUrl = uploadUrl;
            PublicUrl = publicUrl;
        }

        public UploadResponse()
        {
            
        }
    }
}