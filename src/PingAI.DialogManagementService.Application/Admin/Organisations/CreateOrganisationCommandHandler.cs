using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class CreateOrganisationCommandHandler : IRequestHandler<CreateOrganisationCommand, Organisation>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrganisationCommandHandler(
            IOrganisationRepository organisationRepository, IUnitOfWork unitOfWork, IUserRepository userRepository,
            IProjectRepository projectRepository)
        {
            _organisationRepository = organisationRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        public async Task<Organisation> Handle(CreateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var organisationWithSameName =
                await _organisationRepository.FindOrganisationByName(request.Name);
            if (organisationWithSameName != null)
                throw new BadRequestException($"Organisation with name '{request.Name}' already exists. " +
                                              "Please use a different name.");
            var organisationToCreate = await _organisationRepository.AddOrganisation(
                new Organisation(request.Name, string.Empty, null));
            if (!string.IsNullOrEmpty(request.Auth0UserId))
            {
                var user = await _userRepository.GetUserByAuth0Id(request.Auth0UserId!);
                if (user == null)
                    throw new BadRequestException("User does not exist");
                organisationToCreate.AddUser(user!);
            }
            CreateDefaultProject(organisationToCreate);
            
            await _unitOfWork.SaveChanges();
            return organisationToCreate;
        }

        private void CreateDefaultProject(Organisation organisation) 
        {
            var defaultProject = new Project("Default project", organisation.Id,
                Defaults.WidgetTitle, Defaults.WidgetColor, Defaults.WidgetDescription,
                null, Defaults.GreetingMessage, null, ApiKey.GenerateNew(), null);
            foreach (var enquiryEntityName in Defaults.EnquiryEntityNames)
            {
                defaultProject.AddEntityName(new EntityName(enquiryEntityName, Guid.Empty, true));
            }
            organisation.AddProject(defaultProject);
        }
    }
}
