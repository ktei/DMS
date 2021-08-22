using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Webhooks.CreateWebhook
{
    public class CreateWebhookCommandHandler : IRequestHandler<CreateWebhookCommand, CreateWebhookResult>
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;


        public CreateWebhookCommandHandler(IResponseRepository responseRepository,
            IEntityNameRepository entityNameRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService)
        {
            _responseRepository = responseRepository;
            _entityNameRepository = entityNameRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        public async Task<CreateWebhookResult> Handle(CreateWebhookCommand request, CancellationToken cancellationToken)
        {
            var canWriteProject = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWriteProject)
                throw new UnauthorizedException(ErrorDescriptions.ProjectWriteDenied);
            var entityName = request.Name;
            var entityNames = await _entityNameRepository.ListByProjectId(request.ProjectId);
            if (entityNames.Any(n => string.CompareOrdinal(n.Name, entityName) == 0))
            {
                throw new BadRequestException($"Entity name {entityName} already exists.");
            }

            var headers = request.Headers.Select(h =>
                new WebhookHeader(h.Key, h.Value)).ToArray();

            var createdEntityName =
                await _entityNameRepository.Add(new EntityName(request.ProjectId, entityName, true));

            var resolution = Resolution.Factory.Webhook(new WebhookResolution(createdEntityName.Id, request.Name,
                request.Method, request.Url, headers));

            var response = new Response(request.ProjectId, resolution, 
                ResponseType.WEBHOOK, 0); // TODO: what's the order?

            _responseRepository.Remove(response);
            
            await _responseRepository.Add(response);

            await _unitOfWork.SaveChanges();

            return new CreateWebhookResult(createdEntityName, response);
        }
    }
}