using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.GetQuery
{
    public class GetQueryQueryHandler : IRequestHandler<GetQueryQuery, Query>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetQueryQueryHandler(IQueryRepository queryRepository, IAuthorizationService authorizationService)
        {
            _queryRepository = queryRepository;
            _authorizationService = authorizationService;
        }
        
        public async Task<Query> Handle(GetQueryQuery request, CancellationToken cancellationToken)
        {
            var query = await _queryRepository.GetQueryById(request.QueryId);
            if (query == null)
                throw new BadRequestException(QueryNotFound);
            var canWrite = await _authorizationService.UserCanWriteProject(query.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);

            return query;
        }
    }
}