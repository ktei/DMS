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
                await _organisationRepository.FindOrganisationByName(request.Name);
            if (organisationWithSameName != null)
                throw new BadRequestException($"Organisation with name '{request.Name}' already exists. " +
                                              "Please use a different name.");
            var organisation = await _organisationRepository.AddOrganisation(
                new Organisation(request.Name, string.Empty, null));
            if (request.AdminUserId.HasValue)
            {
                var adminUser = await _userRepository.GetUserById(request.AdminUserId.Value);
                if (adminUser == null)
                    throw new BadRequestException("User does not exist");
                organisation.AddUser(adminUser!);

            }
            await _unitOfWork.SaveChanges();
            return organisation;
        }
    }
}