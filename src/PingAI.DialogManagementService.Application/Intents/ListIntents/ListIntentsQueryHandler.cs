using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Intents.ListIntents
{
    public class ListIntentsQueryHandler : IRequestHandler<ListIntentsQuery, List<Intent>>
    {
        private readonly IIntentRepository _intentRepository;
        private readonly IAuthorizationService _authorizationService;

        public ListIntentsQueryHandler(IIntentRepository intentRepository, IAuthorizationService authorizationService)
        {
            _intentRepository = intentRepository;
            _authorizationService = authorizationService;
        }

        public async Task<List<Intent>> Handle(ListIntentsQuery request, CancellationToken cancellationToken)
        {
            if (!request.ProjectId.HasValue)
            {
                return new List<Intent>(0);
            }

            var canRead = await _authorizationService.UserCanReadProject(request.ProjectId.Value);
            if (!canRead)
                throw new ForbiddenException(ProjectReadDenied);

            var results = await _intentRepository.GetIntentsByProjectId(request.ProjectId.Value);
            
            return results;
        }
    }
}