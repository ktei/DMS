using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Intents.CreateIntent
{
    public class CreateIntentCommandHandler : IRequestHandler<CreateIntentCommand, Intent>
    {
        private readonly IIntentRepository _intentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateIntentCommandHandler(IIntentRepository intentRepository, IUnitOfWork unitOfWork)
        {
            _intentRepository = intentRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<Intent> Handle(CreateIntentCommand request, CancellationToken cancellationToken)
        {
            var intent = new Intent(Guid.NewGuid(), request.Name, request.ProjectId, request.IntentType);
            intent = await _intentRepository.AddIntent(intent);
            await _unitOfWork.SaveChanges();
            return intent;
        }
    }
}