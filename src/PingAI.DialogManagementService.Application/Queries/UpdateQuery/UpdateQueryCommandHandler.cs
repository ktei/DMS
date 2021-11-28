using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Application.Queries.Shared;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.UpdateQuery
{
    public class UpdateQueryCommandHandler : IRequestHandler<UpdateQueryCommand, Query>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;
        private readonly INluService _nluService;

        public UpdateQueryCommandHandler(IQueryRepository queryRepository,
            IEntityNameRepository entityNameRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService, INluService nluService, IResponseRepository responseRepository)
        {
            _queryRepository = queryRepository;
            _entityNameRepository = entityNameRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _nluService = nluService;
            _responseRepository = responseRepository;
        }

        public async Task<Query> Handle(UpdateQueryCommand request, CancellationToken cancellationToken)
        {
            var query = await _queryRepository.FindById(request.QueryId);
            if (query == null)
                throw new BadRequestException(QueryNotFound);
            var canWrite = await _authorizationService.UserCanWriteProject(query.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);

            var intent = query.Intents.FirstOrDefault();
            if (intent == null)
            {
                intent = new Intent(query.ProjectId, request.Name, IntentType.STANDARD);
                query.ClearIntents();
                query.AddIntent(intent);
            }

            var entityNames = await _entityNameRepository.ListByProjectId(query.ProjectId);
            intent.ClearPhrases();
            IntentExtensions.AddPhrases(intent, request.PhraseParts, entityNames);
            intent.Rename(request.Name);
            
            var responsesToAdd = await ResponseHelper.CreateResponses(query.ProjectId,
                request.Responses, entityNames, _responseRepository);
            query.ClearResponses();
            foreach (var response in responsesToAdd)
            {
                query.AddResponse(response);
            }

            await _unitOfWork.ExecuteTransaction(() => _nluService.SaveIntent(intent));
            return query;
        }
    }
}