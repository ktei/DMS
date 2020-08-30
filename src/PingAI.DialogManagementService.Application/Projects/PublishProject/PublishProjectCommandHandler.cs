using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Projects.PublishProject
{
    public class PublishProjectCommandHandler : IRequestHandler<PublishProjectCommand, Project>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IRequestContext _requestContext;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectVersionRepository _projectVersionRepository;
        private readonly IUnitOfWork _uow;

        public PublishProjectCommandHandler(IAuthorizationService authorizationService, IRequestContext requestContext,
            IProjectRepository projectRepository, IProjectVersionRepository projectVersionRepository, IUnitOfWork uow)
        {
            _authorizationService = authorizationService;
            _requestContext = requestContext;
            _projectRepository = projectRepository;
            _projectVersionRepository = projectVersionRepository;
            _uow = uow;
        }

        public async Task<Project> Handle(PublishProjectCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            var project = await _projectRepository.GetFullProjectById(request.ProjectId);
            if (project == null)
                throw new ForbiddenException(ProjectReadDenied);
            var user = await _requestContext.GetUser();
            var organisationId = user.OrganisationIds.First();
            var latestVersion = await _projectVersionRepository.GetLatestVersionByProjectId(request.ProjectId);
            if (latestVersion == null)
                throw new InvalidOperationException($"No version found for project {request.ProjectId}");
            var projectToPublish = project.Export();
            var versionToPublish = new ProjectVersion(projectToPublish, organisationId, latestVersion.VersionGroupId,
                latestVersion.Version.Next());
            await _projectVersionRepository.AddProjectVersion(versionToPublish);
            await _uow.SaveChanges();
            return projectToPublish;
        }
    }
}