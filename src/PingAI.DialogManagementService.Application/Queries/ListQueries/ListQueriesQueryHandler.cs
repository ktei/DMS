using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.ListQueries
{
    public class ListQueriesQueryHandler : IRequestHandler<ListQueriesQuery, List<Query>>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IAuthorizationService _authorizationService;

        public ListQueriesQueryHandler(IQueryRepository queryRepository, IAuthorizationService authorizationService)
        {
            _queryRepository = queryRepository;
            _authorizationService = authorizationService;
        }
        
        public async Task<List<Query>> Handle(ListQueriesQuery request, CancellationToken cancellationToken)
        {
            var canRead = await _authorizationService.UserCanReadProject(request.ProjectId);
            if (!canRead)
                throw new ForbiddenException(ProjectReadDenied);

            Expression<Func<Query, bool>>? filter = request.QueryType switch
            {
                // TODO: we shouldn't compare enums with "ToString", but
                // without it the code throws runtime exception complaining
                // it can't find enum_Response_type
                QueryTypes.Faq => q => q.QueryResponses.Any(
                    qr => qr.Response.Type.ToString() == ResponseType.RTE.ToString()),
                QueryTypes.Handover => q => q.QueryResponses.Any(
                    qr => qr.Response.Type.ToString() == ResponseType.HANDOVER.ToString()),
                _ => null
            };
            
            var queries = await _queryRepository.GetQueriesByProjectId(request.ProjectId, filter);
            return queries;
        }
    }
}