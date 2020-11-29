using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.UpdateDisplayOrders
{
    public class UpdateDisplayOrdersCommandHandler : AsyncRequestHandler<UpdateDisplayOrdersCommand>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public UpdateDisplayOrdersCommandHandler(IQueryRepository queryRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService)
        {
            _queryRepository = queryRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }


        protected override async Task Handle(UpdateDisplayOrdersCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);

            var queries = await _queryRepository.GetQueriesByProjectId(request.ProjectId);
            foreach (var query in queries)
            {
                if (request.DisplayOrders.TryGetValue(query.Id, out var displayOrder))
                {
                    query.UpdateDisplayOrder(displayOrder);
                }
            }

            await _unitOfWork.SaveChanges();
        }
    }
}
