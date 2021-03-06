using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.ListQueries
{
    public class ListQueriesQueryHandler : IRequestHandler<ListQueriesQuery, IReadOnlyList<Query>>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IProjectVersionRepository _projectVersionRepository;

        public ListQueriesQueryHandler(IQueryRepository queryRepository, IAuthorizationService authorizationService,
            IProjectVersionRepository projectVersionRepository)
        {
            _queryRepository = queryRepository;
            _authorizationService = authorizationService;
            _projectVersionRepository = projectVersionRepository;
        }

        public async Task<IReadOnlyList<Query>> Handle(ListQueriesQuery request, CancellationToken cancellationToken)
        {
            var projectId = request.ProjectId;
            
            // Some API consumer such as chatbot runtime wants to query
            // the latest published (runtime) project's queries
            if (request.Runtime)
            {
                var latestProjectVersion =
                    await _projectVersionRepository.FindLatestByProjectId(projectId);
                if (latestProjectVersion != null)
                {
                    projectId = latestProjectVersion.ProjectId;
                }
            }
            
            var canRead = await _authorizationService.UserCanReadProject(projectId);
            if (!canRead)
                throw new ForbiddenException(ProjectReadDenied);

            var responseType = request.QueryType switch
            {
                QueryTypes.Faq => ResponseType.RTE,
                QueryTypes.Handover => ResponseType.HANDOVER,
                QueryTypes.Enquiry => ResponseType.FORM,
                _ => default(ResponseType?)
            };
            
            var queries = await _queryRepository.ListByProjectId(projectId, responseType);
            return queries;
        }
    }
}
