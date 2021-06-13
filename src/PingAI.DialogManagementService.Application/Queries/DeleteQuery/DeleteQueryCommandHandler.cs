using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Repositories;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.DeleteQuery
{
    public class DeleteQueryCommandHandler : AsyncRequestHandler<DeleteQueryCommand>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IIntentRepository _intentRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INluService _nluService;

        public DeleteQueryCommandHandler(IQueryRepository queryRepository, IAuthorizationService authorizationService,
            IUnitOfWork unitOfWork, IIntentRepository intentRepository, INluService nluService)
        {
            _queryRepository = queryRepository;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
            _intentRepository = intentRepository;
            _nluService = nluService;
        }

        protected override async Task Handle(DeleteQueryCommand request, CancellationToken cancellationToken)
        {
            var query = await _queryRepository.FindById(request.QueryId);
            if (query == null)
                return;
            var canWrite = await _authorizationService.UserCanWriteProject(query.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);

            var intentsToDelete = query.Intents.ToArray();
            _queryRepository.Remove(query);
            foreach (var intent in intentsToDelete)
            {
                _intentRepository.Remove(intent);
            }

            await _unitOfWork.ExecuteTransaction(() =>
                Task.WhenAll(intentsToDelete.Select(x => _nluService.DeleteIntent(query.ProjectId, x.Id))));
        }
    }
}
