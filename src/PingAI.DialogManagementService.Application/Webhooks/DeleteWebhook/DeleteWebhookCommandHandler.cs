using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Webhooks.DeleteWebhook
{
    public class DeleteWebhookCommandHandler : AsyncRequestHandler<DeleteWebhookCommand>
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityNameRepository _entityNameRepository;

        public DeleteWebhookCommandHandler(IResponseRepository responseRepository,
            IAuthorizationService authorizationService, IUnitOfWork unitOfWork,
            IEntityNameRepository entityNameRepository)
        {
            _responseRepository = responseRepository;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
            _entityNameRepository = entityNameRepository;
        }

        protected override async Task Handle(DeleteWebhookCommand request, CancellationToken cancellationToken)
        {
            var response = await _responseRepository.FindById(request.ResponseId);
            if (response == null)
                throw new NotFoundException(ErrorDescriptions.ResponseNotFound);
            var canWriteProject = await _authorizationService.UserCanWriteProject(response.ProjectId);
            if (!canWriteProject)
                throw new UnauthorizedException(ErrorDescriptions.ProjectWriteDenied);
            var entityName = response.Resolution!.Webhook!.EntityName;
            var entityNameEntity = await _entityNameRepository.FindByName(response.ProjectId, entityName);
            if (entityNameEntity != null)
                _entityNameRepository.Remove(entityNameEntity);
            _responseRepository.Remove(response);
            await _unitOfWork.SaveChanges();
        }
    }
}