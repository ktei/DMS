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
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrganisationCommandHandler(
            IOrganisationRepository organisationRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _organisationRepository = organisationRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<Organisation> Handle(CreateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var organisationWithSameName =
                await _organisationRepository.FindByName(request.Name);
            if (organisationWithSameName != null)
                throw new BadRequestException($"Organisation with name '{request.Name}' already exists. " +
                                              "Please use a different name.");

            var organisation = new Organisation(request.Name, request.Description ?? string.Empty);
            if (!string.IsNullOrEmpty(request.Auth0UserId))
            {
                var user = await _userRepository.GetUserByAuth0Id(request.Auth0UserId!);
                if (user == null)
                    throw new BadRequestException("User does not exist");
                organisation.AddUser(user!);
            }
            var defaultProject = Project.CreateWithDefaults(organisation.Id, $"{organisation.Name} - default project");
            ConfigureDefaultProject(defaultProject);
            organisation.AddProject(defaultProject);
            
            await _organisationRepository.Add(organisation);
            await _unitOfWork.SaveChanges();
            return organisation;
        }

        private static void ConfigureDefaultProject(Project project)
        {
            if (project.Id == Guid.Empty)
                throw new ArgumentException($"{nameof(project)}.Id should not be empty.");
            foreach (var enquiryEntityName in Defaults.EnquiryEntityNames)
            {
                project.AddEntityName(new EntityName(project.Id, enquiryEntityName, true));
            }
        }
    }
}
