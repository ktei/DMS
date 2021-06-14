using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
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
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectVersionRepository _projectVersionRepository;
        private readonly IUnitOfWork _uow;
        private readonly INluService _nluService;

        public PublishProjectCommandHandler(IAuthorizationService authorizationService,
            IProjectRepository projectRepository, IProjectVersionRepository projectVersionRepository, IUnitOfWork uow,
            INluService nluService)
        {
            _authorizationService = authorizationService;
            _projectRepository = projectRepository;
            _projectVersionRepository = projectVersionRepository;
            _uow = uow;
            _nluService = nluService;
        }

        public async Task<Project> Handle(PublishProjectCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            
            var sourceProject = await PerformanceLogger.Monitor(() => _projectRepository.FindByIdWithJoins(request.ProjectId),
                nameof(_projectRepository.FindByIdWithJoins));
            if (sourceProject == null)
                throw new ForbiddenException(ProjectReadDenied);
            
            var validationErrors = Validate(sourceProject);
            if (validationErrors.Any())
                throw new BadRequestException($"Publish failed. {string.Join(" ", validationErrors)}");
            
            var organisationId = sourceProject.OrganisationId;
            var latestVersion = await _projectVersionRepository.FindLatestByProjectId(request.ProjectId);
            if (latestVersion == null)
                throw new InvalidOperationException($"No version found for project {request.ProjectId}");
            var targetProject =
                Project.CreateWithDefaults(organisationId, $"{sourceProject.Name}__{DateTime.UtcNow.Ticks}");
            await _projectRepository.Add(targetProject);
            targetProject.Import(sourceProject);
            var versionToPublish = latestVersion.Next(targetProject.Id);
            await _projectVersionRepository.Add(versionToPublish);

            await _uow.ExecuteTransaction(async () =>
            {
                // Export source project's NLU data
                await PerformanceLogger.Monitor(() => _nluService.Export(sourceProject.Id,
                    targetProject.Id), "NluService.Export");

                // The exported NLU data in wrapped in target project, we want to apply that
                // data to source project's runtime bot. Hence, the target is source project
                // and the source is the target project. The logic is a bit twisted here.
                await PerformanceLogger.Monitor(
                    () => _nluService.Import(targetProject.Id, sourceProject.Id),
                    "NluService.Import");
            });

            return targetProject;
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
