using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Webhooks.UpdateWebhook
{
    public class UpdateWebhookCommandHandler : IRequestHandler<UpdateWebhookCommand, Response>
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityNameRepository _entityNameRepository;

        public UpdateWebhookCommandHandler(IResponseRepository responseRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService, IEntityNameRepository entityNameRepository)
        {
            _responseRepository = responseRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _entityNameRepository = entityNameRepository;
        }

        public async Task<Response> Handle(UpdateWebhookCommand request, CancellationToken cancellationToken)
        {
            var response = await _responseRepository.FindById(request.ResponseId);
            if (response == null)
                throw new NotFoundException(ErrorDescriptions.ResponseNotFound);
            var canWriteProject = await _authorizationService.UserCanWriteProject(response.ProjectId);
            if (!canWriteProject)
                throw new UnauthorizedException(ErrorDescriptions.ProjectWriteDenied);

            var entityNameId =
                await _entityNameRepository.FindByName(response.ProjectId, response.Resolution!.Webhook!.EntityName);

            var webhook = new WebhookResolution(entityNameId!.Id, response.Resolution!.Webhook!.EntityName,
                request.Method, request.Url, request.Headers.Select(h => new WebhookHeader(h.Key, h.Value)).ToArray());
            response.SetWebhook(webhook);
            await _unitOfWork.SaveChanges();
            return response;
        }
    }
}