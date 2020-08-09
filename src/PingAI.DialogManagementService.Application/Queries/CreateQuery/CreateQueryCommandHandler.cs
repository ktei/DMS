using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.CreateQuery
{
    public class CreateQueryCommandHandler : IRequestHandler<CreateQueryCommand, Query>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IIntentRepository _intentRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public CreateQueryCommandHandler(IQueryRepository queryRepository, IIntentRepository intentRepository,
            IResponseRepository responseRepository, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
        {
            _queryRepository = queryRepository;
            _intentRepository = intentRepository;
            _responseRepository = responseRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        public async Task<Query> Handle(CreateQueryCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ErrorDescriptions.ProjectWriteDenied);

            var query = new Query(request.Name, request.ProjectId, request.Expressions,
                request.Description, request.Tags, request.DisplayOrder);
            if (request.IntentId.HasValue)
            {
                var intent = await _intentRepository.GetIntentById(request.IntentId.Value);
                if (intent == null) throw new BadRequestException(ErrorDescriptions.IntentNotFound);
            }
            else if (request.Intent != null)
            {
                query.AddIntent(request.Intent);
            }
            else
            {
                throw new BadRequestException("Missing intent or intentId, one of them should be specified.");
            }

            if (request.ResponseId.HasValue)
            {
                var response = await _responseRepository.GetResponseById(request.ResponseId.Value);
                if (response == null) throw new BadRequestException(ErrorDescriptions.ResponseNotFound);
            }
            else if (request.Response != null)
            {
                query.AddResponse(request.Response);
            }
            else
            {
                throw new BadRequestException("Missing response or responseId, one of them should be specified.");
            }

            query = await _queryRepository.AddQuery(query);
            await _unitOfWork.SaveChanges();
            return query;
        }
    }
}