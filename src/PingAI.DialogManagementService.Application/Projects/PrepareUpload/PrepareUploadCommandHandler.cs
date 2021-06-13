using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Configuration;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Application.Interfaces.Services.Storage;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Projects.PrepareUpload
{
    public class PrepareUploadCommandHandler : IRequestHandler<PrepareUploadCommand, PrepareUploadCommandResult>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IS3Service _s3Service;
        private readonly IAppConfig _appConfig;

        public PrepareUploadCommandHandler(IAuthorizationService authorizationService, IS3Service s3Service,
            IAppConfig appConfig)
        {
            _authorizationService = authorizationService;
            _s3Service = s3Service;
            _appConfig = appConfig;
        }

        public async Task<PrepareUploadCommandResult> Handle(PrepareUploadCommand request,
            CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);

            var key = MakeObjectKey(request.ProjectId, request.FileName);
            var uploadUrl = _s3Service.GetPreSignedUploadUrl(_appConfig.BucketName,
                request.ContentType, key);
            var publicUrl = MakePublicUrl(key);

            return new PrepareUploadCommandResult(uploadUrl, publicUrl);
        }

        private string MakePublicUrl(string objectKey)
        {
            if (string.IsNullOrEmpty(_appConfig.PublicBaseUrl))
                throw new InvalidOperationException($"config[{nameof(_appConfig.PublicBaseUrl)}] is empty.");
            
            return $"{_appConfig.PublicBaseUrl}/{objectKey.Substring("public/".Length)}";
        }

        private static string MakeObjectKey(Guid projectId, string fileName)
        {
            return $"public/projects/{projectId}/files/{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        }
    }
}
