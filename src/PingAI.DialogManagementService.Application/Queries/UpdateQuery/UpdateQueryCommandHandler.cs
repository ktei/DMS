using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.UpdateQuery
{
    public class UpdateQueryCommandHandler : IRequestHandler<UpdateQueryCommand, Query>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IIntentRepository _intentRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public UpdateQueryCommandHandler(IQueryRepository queryRepository, IIntentRepository intentRepository,
            IResponseRepository responseRepository, IEntityNameRepository entityNameRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService)
        {
            _queryRepository = queryRepository;
            _intentRepository = intentRepository;
            _responseRepository = responseRepository;
            _entityNameRepository = entityNameRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        public async Task<Query> Handle(UpdateQueryCommand request, CancellationToken cancellationToken)
        {
            var query = await _queryRepository.GetQueryById(request.QueryId);
            if (query == null)
                throw new BadRequestException(QueryNotFound);
            var canWrite = await _authorizationService.UserCanWriteProject(query.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            
            if (request.IntentId.HasValue)
            {
                var intent = await _intentRepository.GetIntentById(request.IntentId.Value);
                if (intent == null) throw new BadRequestException(IntentNotFound);
                query.ClearIntents();
                query.AddIntent(intent);
            }
            else if (request.Intent != null)
            {
                // TODO: this won't delete the detached intent
                query.ClearIntents();
                query.AddIntent(new Intent(request.Intent.Name, query.ProjectId, request.Intent.Type));
            }
            else
            {
                throw new BadRequestException("Missing intent and intentId, one of them should be specified.");
            }
            
            if (request.ResponseId.HasValue)
            {
                var response = await _responseRepository.GetResponseById(request.ResponseId.Value);
                if (response == null) throw new BadRequestException(ResponseNotFound);
                query.ClearResponses();
                query.AddResponse(response);
            }
            else if (request.Response != null)
            {
                // TODO: we only support RTE for now
                var entityNames = await _entityNameRepository.GetEntityNamesByProjectId(query.ProjectId);
                Debug.Assert(!string.IsNullOrEmpty(request.RteText));
                request.Response.SetRteText(request.RteText!, entityNames.ToDictionary(x => x.Name));
                
                // TODO: this won't delete the detached response though
                query.ClearResponses();
                query.AddResponse(new Response(request.Response.Resolution,
                    query.ProjectId, request.Response.Type, request.Response.Order));
            }
            else
            {
                throw new BadRequestException("Missing response and responseId, one of them should be specified.");
            }
            
            await _unitOfWork.SaveChanges();
            return query;
        }
    }
}