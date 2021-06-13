using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Application.Utils;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Projects.PublishProject
{
    public class PublishProjectCommandHandler : IRequestHandler<PublishProjectCommand, Project>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityContext _identityContext;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectVersionRepository _projectVersionRepository;
        private readonly IUnitOfWork _uow;

        public PublishProjectCommandHandler(IAuthorizationService authorizationService, IIdentityContext identityContext,
            IProjectRepository projectRepository, IProjectVersionRepository projectVersionRepository, IUnitOfWork uow)
        {
            _authorizationService = authorizationService;
            _identityContext = identityContext;
            _projectRepository = projectRepository;
            _projectVersionRepository = projectVersionRepository;
            _uow = uow;
        }

        public async Task<Project> Handle(PublishProjectCommand request, CancellationToken cancellationToken)
        {
            // TODO:
            throw new NotImplementedException();
            // var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            // if (!canWrite)
            //     throw new ForbiddenException(ProjectWriteDenied);
            //
            // var project = await PerformanceLogger.Monitor(_projectRepository.GetFullProjectById(request.ProjectId),
            //     nameof(_projectRepository.GetFullProjectById));
            // if (project == null)
            //     throw new ForbiddenException(ProjectReadDenied);
            //
            // var validationErrors = Validate(project);
            // if (validationErrors.Any())
            //     throw new BadRequestException($"Publish failed. {string.Join(" ", validationErrors)}");
            //
            // var user = await _requestContext.GetUser();
            // var organisationId = project.OrganisationId; // user.OrganisationIds.First();
            // var latestVersion = await _projectVersionRepository.GetLatestVersionByProjectId(request.ProjectId);
            // if (latestVersion == null)
            //     throw new InvalidOperationException($"No version found for project {request.ProjectId}");
            // var projectToPublish = project.Export();
            // var versionToPublish = new ProjectVersion(projectToPublish, organisationId, latestVersion.VersionGroupId,
            //     latestVersion.Version.Next());
            // await _projectVersionRepository.AddProjectVersion(versionToPublish);
            // await _uow.SaveChanges();
            // return projectToPublish;
        }

        /// <summary>
        /// Validate if the project can be published
        /// </summary>
        /// <param name="project"></param>
        private static string[] Validate(Project project)
        {
            var errors = new List<string>();
            if (project.Domains == null || !project.Domains.Any())
            {
                errors.Add("Please configure a domain.");
            }

            if (string.IsNullOrEmpty(project.BusinessEmail))
            {
                errors.Add("Please configure business email.");
            }

            return errors.ToArray();
        }
    }
}