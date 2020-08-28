using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.CreateProject
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project>
    {
        private readonly IRequestContext _requestContext;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _uow;

        public CreateProjectCommandHandler(
            IRequestContext requestContext, IOrganisationRepository organisationRepository, IUnitOfWork uow,
            IProjectRepository projectRepository)
        {
            _requestContext = requestContext;
            _organisationRepository = organisationRepository;
            _uow = uow;
            _projectRepository = projectRepository;
        }

        public async Task<Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var user = await _requestContext.GetUser();
            if (user.OrganisationIds.Any())
                throw new BadRequestException("Missing organisationId");

            var organisationId = user.OrganisationIds.First();
            var organisation = (await _organisationRepository
                .GetOrganisationsByIds(new[] {organisationId})).FirstOrDefault();
            if (organisation == null)
                throw new BadRequestException("No organisation found");
            
            // ensure project name does not duplicate
            var nameExists = await _projectRepository.ProjectNameExists(organisation.Id, request.Name);
            if (nameExists)
                throw new BadRequestException($"Project with same name '{request.Name}' already exists");

            var project = new Project(request.Name, organisation.Id,
                null, Defaults.WidgetColor, null, 
                null, null, null,
                ApiKey.Empty, null);
            organisation.AddProjectVersion(new ProjectVersion(project, organisationId, 
                Guid.NewGuid(), ProjectVersionNumber.NewDesignTime()));

            await _uow.SaveChanges();

            return project;
        }
    }
}