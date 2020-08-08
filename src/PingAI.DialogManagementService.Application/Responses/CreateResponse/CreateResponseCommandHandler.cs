using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Responses.CreateResponse
{
    public class CreateResponseCommandHandler : IRequestHandler<CreateResponseCommand, Response>
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public CreateResponseCommandHandler(IResponseRepository responseRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService, IEntityNameRepository entityNameRepository)
        {
            _responseRepository = responseRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _entityNameRepository = entityNameRepository;
        }

        public async Task<Response> Handle(CreateResponseCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanReadProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ErrorDescriptions.ProjectWriteDenied);
            var entityNames = await _entityNameRepository.GetEntityNamesByProjectId(request.ProjectId);
            var response = new Response(Guid.NewGuid(), new ResolutionPart[0], request.ProjectId,
                request.Type, request.Order);
            
            // TODO: remove this assert after we support more types of responses
            Debug.Assert(request.Type == ResponseType.RTE);
            if (request.Type == ResponseType.RTE)
            {
                Debug.Assert(request.RteText != null);
                response.SetRte(request.RteText!, entityNames.ToDictionary(e => e.Name));
            }

            response = await _responseRepository.AddResponse(response);
            await _unitOfWork.SaveChanges();
            return response;
        }
    }
}