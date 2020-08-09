using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Intents.GetIntent
{
    public class GetIntentQueryHandler : IRequestHandler<GetIntentQuery, Intent>
    {
        private readonly IIntentRepository _intentRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetIntentQueryHandler(IIntentRepository intentRepository, IAuthorizationService authorizationService)
        {
            _intentRepository = intentRepository;
            _authorizationService = authorizationService;
        }
        
        public async Task<Intent> Handle(GetIntentQuery request, CancellationToken cancellationToken)
        {
            var intent = await _intentRepository.GetIntentById(request.IntentId);
            if (intent == null)
            {
                throw new NotFoundException(IntentNotFound);
            }

            var canRead = await _authorizationService.UserCanReadProject(intent.ProjectId);
            if (!canRead)
                throw new ForbiddenException(ProjectReadDenied);

            return intent;
        }
    }
}