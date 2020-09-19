using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class ListOrganisationsQueryHandler : IRequestHandler<ListOrganisationsQuery, List<Organisation>>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IUserRepository _userRepository;

        public ListOrganisationsQueryHandler(IOrganisationRepository organisationRepository,
            IUserRepository userRepository)
        {
            _organisationRepository = organisationRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Organisation>> Handle(ListOrganisationsQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Auth0UserId))
            {
                var user = await _userRepository.GetUserByAuth0Id(request.Auth0UserId!);
                if (user == null)
                    throw new BadRequestException("User does not exist");
                return await _organisationRepository.GetOrganisationsByUserId(user.Id);
            }

            return await _organisationRepository.GetAllOrganisations();
        }
    }
}