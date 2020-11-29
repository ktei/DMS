using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Queries.Insert
{
    public class InsertCommandHandler : AsyncRequestHandler<InsertCommand>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public InsertCommandHandler(IQueryRepository queryRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService)
        {
            _queryRepository = queryRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }


        protected override async Task Handle(InsertCommand request, CancellationToken cancellationToken)
        {
            var query = await _queryRepository.GetQueryByIdWithoutJoins(request.QueryId);
            if (query == null)
                throw new BadRequestException(QueryNotFound);

            var canWrite = await _authorizationService.UserCanWriteProject(query.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);

            var queries = await _queryRepository.GetQueriesByProjectId(query.ProjectId);
            query.Insert(queries, request.DisplayOrder);
            
            await _unitOfWork.SaveChanges();
        }
    }
}