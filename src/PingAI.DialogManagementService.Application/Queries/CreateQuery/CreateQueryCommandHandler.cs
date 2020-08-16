using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.CreateQuery
{
    public class CreateQueryCommandHandler : IRequestHandler<CreateQueryCommand, Query>
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IIntentRepository _intentRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IEntityTypeRepository _entityTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public CreateQueryCommandHandler(IQueryRepository queryRepository, IIntentRepository intentRepository,
            IResponseRepository responseRepository, IUnitOfWork unitOfWork, IAuthorizationService authorizationService,
            IEntityNameRepository entityNameRepository, IEntityTypeRepository entityTypeRepository)
        {
            _queryRepository = queryRepository;
            _intentRepository = intentRepository;
            _responseRepository = responseRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _entityNameRepository = entityNameRepository;
            _entityTypeRepository = entityTypeRepository;
        }

        public async Task<Query> Handle(CreateQueryCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ErrorDescriptions.ProjectWriteDenied);

            var nextDisplayOrder = (await _queryRepository.GetMaxDisplayOrder(request.ProjectId)) + 1;
            var query = new Query(request.Name, request.ProjectId, request.Expressions,
                request.Description, request.Tags, nextDisplayOrder);
            if (request.IntentId.HasValue)
            {
                var intent = await _intentRepository.GetIntentById(request.IntentId.Value);
                if (intent == null) throw new BadRequestException(ErrorDescriptions.IntentNotFound);
                query.AddIntent(intent);
            }
            else if (request.Intent != null)
            {
                var entityNames = await _entityNameRepository.GetEntityNamesByProjectId(request.ProjectId);
                var entityTypes = await _entityTypeRepository.GetEntityTypesByProjectId(request.ProjectId);
                var entityTypeIds = entityTypes.Select(e => e.Id).ToArray();
                var entityNamesToCreate = new List<EntityName>();
                
                foreach (var phrasePart in request.Intent.PhraseParts)
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
                
                query.AddIntent(new Intent(request.Intent.Name, query.ProjectId, request.Intent.Type,
                    request.Intent.PhraseParts));
            }
            else
            {
                throw new BadRequestException("Missing intent and intentId, one of them should be specified.");
            }

            if (request.ResponseId.HasValue)
            {
                var response = await _responseRepository.GetResponseById(request.ResponseId.Value);
                if (response == null) throw new BadRequestException(ErrorDescriptions.ResponseNotFound);
                query.AddResponse(response);
            }
            else if (request.Response != null)
            {
                // TODO: we only support RTE for now
                var entityNames = await _entityNameRepository.GetEntityNamesByProjectId(request.ProjectId);
                Debug.Assert(!string.IsNullOrEmpty(request.RteText));
                request.Response.SetRteText(request.RteText!, entityNames.ToDictionary(x => x.Name));
                query.AddResponse(new Response(request.Response.Resolution,
                    query.ProjectId, request.Response.Type, request.Response.Order));
            }
            else
            {
                throw new BadRequestException("Missing response and responseId, one of them should be specified.");
            }

            query = await _queryRepository.AddQuery(query);
            await _unitOfWork.SaveChanges();
            return query;
        }
    }
}