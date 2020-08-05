using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Intents.CreateIntent
{
    public class CreateIntentCommandHandler : IRequestHandler<CreateIntentCommand, Intent>
    {
        private readonly IIntentRepository _intentRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public CreateIntentCommandHandler(IIntentRepository intentRepository, IUnitOfWork unitOfWork,
            IAuthService authService, IProjectRepository projectRepository)
        {
            _intentRepository = intentRepository;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _projectRepository = projectRepository;
        }

        public async Task<Intent> Handle(CreateIntentCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException("No permission to create intents under current project");
            
            var intent = new Intent(Guid.NewGuid(), request.Name, request.ProjectId, request.IntentType);
            intent = await _intentRepository.AddIntent(intent);
            await _unitOfWork.SaveChanges();
            return intent;
        }
    }
}