using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class ListOrganisationsQueryHandler : IRequestHandler<ListOrganisationsQuery, List<Organisation>>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public ListOrganisationsQueryHandler(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }
        
        public Task<List<Organisation>> Handle(ListOrganisationsQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId.HasValue)
                return _organisationRepository.GetOrganisationsByUserId(request.UserId.Value);
            return _organisationRepository.GetAllOrganisations();
        }
    }
}