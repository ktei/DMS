using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.DeleteQuery
{
    public class DeleteQueryCommandHandler : AsyncRequestHandler<DeleteQueryCommand>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQueryCommandHandler(IQueryRepository queryRepository, IAuthorizationService authorizationService,
            IUnitOfWork unitOfWork)
        {
            _queryRepository = queryRepository;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Handle(DeleteQueryCommand request, CancellationToken cancellationToken)
        {
            var query = await _queryRepository.GetQueryById(request.QueryId);
            if (query == null)
                return;
            var canWrite = await _authorizationService.UserCanWriteProject(query.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            
            query.Delete();
            _queryRepository.RemoveQuery(query);

            await _unitOfWork.SaveChanges();
        }
    }
}