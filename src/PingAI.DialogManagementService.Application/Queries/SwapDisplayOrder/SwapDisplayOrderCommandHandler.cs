using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;


namespace PingAI.DialogManagementService.Application.Queries.SwapDisplayOrder
{
    public class SwapDisplayOrderCommandHandler : AsyncRequestHandler<SwapDisplayOrderCommand>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;


        public SwapDisplayOrderCommandHandler(IQueryRepository queryRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService)
        {
            _queryRepository = queryRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        protected override async Task Handle(SwapDisplayOrderCommand request, CancellationToken cancellationToken)
        {
            var query = await _queryRepository.GetQueryByIdWithoutJoins(request.QueryId);
            var targetQuery = await _queryRepository.GetQueryByIdWithoutJoins(request.TargetQueryId);
            if (query == null || targetQuery == null)
                throw new BadRequestException(QueryNotFound);
            
            var canWrite = await _authorizationService.UserCanWriteProject(query.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            
            query.Swap(targetQuery);

            await _unitOfWork.SaveChanges();
        }
    }
}