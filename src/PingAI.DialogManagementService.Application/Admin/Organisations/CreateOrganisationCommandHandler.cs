using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class CreateOrganisationCommandHandler : IRequestHandler<CreateOrganisationCommand, Organisation>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectVersionRepository _projectVersionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrganisationCommandHandler(
            IOrganisationRepository organisationRepository, IUnitOfWork unitOfWork, IUserRepository userRepository,
            IProjectRepository projectRepository, IProjectVersionRepository projectVersionRepository)
        {
            _organisationRepository = organisationRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _projectVersionRepository = projectVersionRepository;
        }

        public async Task<Organisation> Handle(CreateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var organisationWithSameName =
                await _organisationRepository.FindByName(request.Name);
            if (organisationWithSameName != null)
                throw new BadRequestException($"Organisation with name '{request.Name}' already exists. " +
                                              "Please use a different name.");
            
            var organisationToCreate = await _organisationRepository.Add(
                new Organisation(request.Name, request.Description ?? string.Empty));
            if (!string.IsNullOrEmpty(request.Auth0UserId))
            {
                var user = await _userRepository.GetUserByAuth0Id(request.Auth0UserId!);
                if (user == null)
                    throw new BadRequestException("User does not exist");
                organisationToCreate.AddUser(user!);
            }
            var defaultProject = await AddDefaultProject(organisationToCreate);
            ConfigureDefaultProject(defaultProject);
            // await _projectVersionRepository.AddProjectVersion(new ProjectVersion(defaultProject,
            //     organisationToCreate.Id, defaultProject.Id, ProjectVersionNumber.NewDesignTime()));
            
            await _unitOfWork.SaveChanges();
            return organisationToCreate;
        }

        private async Task<Project> AddDefaultProject(Organisation organisation)
        {
            var defaultProject = Project.CreateWithDefaults(organisation.Id);
            await _projectRepository.Add(defaultProject);
            return defaultProject;
        }

        private static void ConfigureDefaultProject(Project project)
        {
            if (project.Id == Guid.Empty)
                throw new ArgumentException($"{nameof(project)}.Id should not be empty.");
            var defaultGreeting = new Response(project.Id, Resolution.Factory.RteText(Defaults.GreetingMessage), 
                ResponseType.RTE, 0);
            project.UpdateGreetingResponses(new[]
            {
                defaultGreeting
            });
            foreach (var enquiryEntityName in Defaults.EnquiryEntityNames)
            {
                project.AddEntityName(new EntityName(project.Id, enquiryEntityName, true));
            }
        }
    }
}
