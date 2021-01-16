namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UploadRequest
    {
        public string FileName { get; set; }

        public UploadRequest(string fileName)
        {
            FileName = fileName;
        }

        public UploadRequest()
        {
            
        }
    }
}