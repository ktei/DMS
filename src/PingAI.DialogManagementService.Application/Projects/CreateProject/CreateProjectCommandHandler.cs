using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Projects.CreateProject
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project>
    {
        private readonly IIdentityContext _identityContext;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _uow;

        public CreateProjectCommandHandler(
            IIdentityContext identityContext, IOrganisationRepository organisationRepository, IUnitOfWork uow,
            IProjectRepository projectRepository)
        {
            _identityContext = identityContext;
            _organisationRepository = organisationRepository;
            _uow = uow;
            _projectRepository = projectRepository;
        }

        public async Task<Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityContext.GetUser();
            if (!user.Organisations.Any())
                throw new BadRequestException($"User {user.Id} has no organisations");
           
            // TODO: if user has multiple organisations, which one is it?
            var organisation = user.Organisations.First();
            
            // ensure project name does not duplicate
            var nameExists = organisation.Projects.Any(x => x.Name == request.Name);
            if (nameExists)
                throw new BadRequestException($"Project with same name '{request.Name}' already exists");

            var project = Project.CreateWithDefaults(organisation.Id, $"{organisation.Name} - default project");
            await _projectRepository.Add(project);
            // organisation.AddProjectVersion(new ProjectVersion(project, organisation.Id, 
            //     Guid.NewGuid(), ProjectVersionNumber.NewDesignTime()));

            await _uow.SaveChanges();

            return project;
        }
    }
}