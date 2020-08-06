using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;
using PhrasePart = PingAI.DialogManagementService.Application.Interfaces.Services.Nlu.PhrasePart;

namespace PingAI.DialogManagementService.Application.Intents.CreateIntent
{
    public class CreateIntentCommandHandler : IRequestHandler<CreateIntentCommand, Intent>
    {
        private readonly IIntentRepository _intentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IEntityTypeRepository _entityTypeRepository;
        private readonly INluService _nluService;

        public CreateIntentCommandHandler(IIntentRepository intentRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService, IEntityNameRepository entityNameRepository,
            IEntityTypeRepository entityTypeRepository, INluService nluService)
        {
            _intentRepository = intentRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _entityNameRepository = entityNameRepository;
            _entityTypeRepository = entityTypeRepository;
            _nluService = nluService;
        }

        public async Task<Intent> Handle(CreateIntentCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            
            var intent = new Intent(Guid.NewGuid(), request.Name, request.ProjectId, request.IntentType);
            var entityNames = await _entityNameRepository.GetEntityNamesByProjectId(request.ProjectId);
            var entityTypes = await _entityTypeRepository.GetEntityTypesByProjectId(request.ProjectId);
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
                        var newEntityName = new EntityName(Guid.NewGuid(), phrasePart.EntityName.Name,
                            request.ProjectId, true);
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
            intent = await _intentRepository.AddIntent(intent);
            
            await _nluService.SaveIntent(new SaveIntentRequest
            {
                IntentId = intent.Id,
                Name = intent.Name,
                ProjectId = intent.ProjectId,
                TrainingPhrases = intent.PhraseParts.GroupBy(p => p.PhraseId)
                    .Select(g => new TrainingPhrase
                    {
                        Parts = g.Select(p => new PhrasePart
                        {
                            EntityName = p.EntityName?.Name,
                            EntityType = p.EntityType?.Name,
                            EntityValue = p.Value,
                            Text = p.Text,
                            Type = p.Type.ToString()
                        }).ToList()
                    }).ToList()
            });
            
            await _unitOfWork.SaveChanges();
            return intent;
        }
    }
}