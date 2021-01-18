namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UploadRequest
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public UploadRequest(string fileName, string contentType)
        {
            FileName = fileName;
            ContentType = contentType;
        }

        public UploadRequest()
        {
            
        }
    }
}