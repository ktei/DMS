using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Queries.Shared;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.CreateQuery
{
    public class CreateQueryCommandHandler : IRequestHandler<CreateQueryCommand, Query>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;
        private readonly INluService _nluService;

        public CreateQueryCommandHandler(IQueryRepository queryRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            IEntityNameRepository entityNameRepository, INluService nluService)
        {
            _queryRepository = queryRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _entityNameRepository = entityNameRepository;
            _nluService = nluService;
        }

        public async Task<Query> Handle(CreateQueryCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ErrorDescriptions.ProjectWriteDenied);

            var displayOrder = (await _queryRepository.GetMaxDisplayOrder(request.ProjectId)) + 1;
            var query = new Query(request.ProjectId, request.Name, request.Expressions,
                request.Description, request.Tags, displayOrder);

            var entityNames = await _entityNameRepository.ListByProjectId(request.ProjectId);
            var intent = new Intent(request.ProjectId, request.Name, IntentType.STANDARD);
            IntentHelper.AddPhrases(intent, request.PhraseParts, entityNames);
            query.AddIntent(intent);

            foreach (var response in ResponseHelper.CreateResponses(request.ProjectId, request.Responses, entityNames))
            {
                query.AddResponse(response);
            }

            await _queryRepository.Add(query);
            
            await _unitOfWork.ExecuteTransaction(() => _nluService.SaveIntent(intent));
            return query;
        }
    }
}
