using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Intents.UpdateIntent
{
    public class UpdateIntentCommandHandler : IRequestHandler<UpdateIntentCommand, Intent>
    {
        private readonly IIntentRepository _intentRepository;
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IEntityTypeRepository _entityTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public UpdateIntentCommandHandler(IIntentRepository intentRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService, IEntityNameRepository entityNameRepository,
            IEntityTypeRepository entityTypeRepository)
        {
            _intentRepository = intentRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _entityNameRepository = entityNameRepository;
            _entityTypeRepository = entityTypeRepository;
        }

        public async Task<Intent> Handle(UpdateIntentCommand request, CancellationToken cancellationToken)
        {
            var intent = await _intentRepository.GetIntentById(request.IntentId);
            if (intent == null)
                throw new BadRequestException(IntentNotFound);
            var canRead = await _authorizationService.UserCanWriteProject(intent.ProjectId);
            if (!canRead)
                throw new ForbiddenException(ProjectReadDenied);
            
            intent.UpdateName(request.Name);
            
            // only update phrase parts if it's not null, used 
            // for name-only update
            if (request.PhraseParts != null)
            {
                var entityNames = await _entityNameRepository.GetEntityNamesByProjectId(intent.ProjectId);
                var entityTypes = await _entityTypeRepository.GetEntityTypesByProjectId(intent.ProjectId);
                var entityTypeIds = entityTypes.Select(e => e.Id).ToArray();
                var entityNamesToCreate = new List<EntityName>();
                foreach (var phrasePart in request.PhraseParts)
                {
                    if (phrasePart.EntityName != null)
                    {
                        var existingEntityName = entityNames.FirstOrDefault(e =>
                            e.Name == phrasePart.EntityName.Name);
                        if (existingEntityName != null)
                        {
                            phrasePart.UpdateEntityName(existingEntityName);
                        }
                        else
                        {
                            var newEntityName = new EntityName(phrasePart.EntityName.Name,
                                intent.ProjectId, true);
                            phrasePart.UpdateEntityName(newEntityName);
                            entityNamesToCreate.Add(newEntityName);
                        }
                    }

                    if (phrasePart.EntityTypeId.HasValue && !entityTypeIds.Contains(phrasePart.EntityTypeId.Value))
                    {
                        throw new BadRequestException(ErrorDescriptions.EntityTypeNotFound);
                    }
                }

                foreach (var newEntityName in entityNamesToCreate)
                {
                    await _entityNameRepository.AddEntityName(newEntityName);
                }
                
                intent.UpdatePhrases(request.PhraseParts);
            }

            await _unitOfWork.SaveChanges();
            return intent;
        }
    }
}