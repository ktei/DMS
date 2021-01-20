using System.Collections.Generic;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Storage
{
    public interface IS3Service
    {
        string GetPreSignedUploadUrl(string bucket, string contentType, string key, 
            Dictionary<string, string>? tags = null);
    }
}