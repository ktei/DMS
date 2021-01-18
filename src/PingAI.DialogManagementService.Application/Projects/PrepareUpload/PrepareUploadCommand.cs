using System;
using MediatR;

namespace PingAI.DialogManagementService.Application.Projects.PrepareUpload
{
    public class PrepareUploadCommand : IRequest<PrepareUploadCommandResult>
    {
        public Guid ProjectId { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }

        public PrepareUploadCommand(Guid projectId, string contentType, string fileName)
        {
            ProjectId = projectId;
            ContentType = contentType;
            FileName = fileName;
        }

        public PrepareUploadCommand()
        {
            
        }
    }

    public class PrepareUploadCommandResult
    {
        public string UploadUrl { get; set; }

        public PrepareUploadCommandResult(string uploadUrl)
        {
            UploadUrl = uploadUrl;
        }

        public PrepareUploadCommandResult()
        {
            
        }
    }
}