// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using MediatR;
// using PingAI.DialogManagementService.Application.Interfaces.Persistence;
// using PingAI.DialogManagementService.Application.Interfaces.Services;
// using PingAI.DialogManagementService.Domain.ErrorHandling;
// using PingAI.DialogManagementService.Domain.Model;
// using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;
//
// namespace PingAI.DialogManagementService.Application.Queries.UpdateQuery
// {
//     public class UpdateQueryCommandHandler : IRequestHandler<UpdateQueryCommand, Query>
//     {
//         private readonly IQueryRepository _queryRepository;
//         private readonly IIntentRepository _intentRepository;
//         private readonly IResponseRepository _responseRepository;
//         private readonly IEntityNameRepository _entityNameRepository;
//         private readonly IEntityTypeRepository _entityTypeRepository;
//         private readonly IUnitOfWork _unitOfWork;
//         private readonly IAuthorizationService _authorizationService;
//
//         public UpdateQueryCommandHandler(IQueryRepository queryRepository, IIntentRepository intentRepository,
//             IResponseRepository responseRepository, IEntityNameRepository entityNameRepository, IUnitOfWork unitOfWork,
//             IAuthorizationService authorizationService, IEntityTypeRepository entityTypeRepository)
//         {
//             _queryRepository = queryRepository;
//             _intentRepository = intentRepository;
//             _responseRepository = responseRepository;
//             _entityNameRepository = entityNameRepository;
//             _unitOfWork = unitOfWork;
//             _authorizationService = authorizationService;
//             _entityTypeRepository = entityTypeRepository;
//         }
//
//         public async Task<Query> Handle(UpdateQueryCommand request, CancellationToken cancellationToken)
//         {
//             var query = await _queryRepository.GetQueryById(request.QueryId);
//             if (query == null)
//                 throw new BadRequestException(QueryNotFound);
//             var canWrite = await _authorizationService.UserCanWriteProject(query.ProjectId);
//             if (!canWrite)
//                 throw new ForbiddenException(ProjectWriteDenied);
//             
//             if (request.IntentId.HasValue)
//             {
//                 var intent = await _intentRepository.GetIntentById(request.IntentId.Value);
//                 if (intent == null) throw new BadRequestException(IntentNotFound);
//                 query.ClearIntents();
//                 query.AddIntent(intent);
//             }
//             else if (request.Intent != null)
//             {
//                 // TODO: this won't delete the detached intent
//                 var existingIntent = query.Intents.FirstOrDefault(i => i.Name == request.Intent.Name);
//                 
//                 var entityNames = await _entityNameRepository.ListByProjectId(query.ProjectId);
//                 var entityTypes = await _entityTypeRepository.ListByProjectId(query.ProjectId);
//                 var entityTypeIds = entityTypes.Select(e => e.Id).ToArray();
//                 var entityNamesToCreate = new List<EntityName>();
//                 
//                 foreach (var phrasePart in request.Intent.PhraseParts)
//                 {
//                     if (phrasePart.EntityName != null)
//                     {
//                         var existingEntityName = entityNames.FirstOrDefault(e => 
//                             e.Name == phrasePart.EntityName.Name);
//                         if (existingEntityName != null)
//                         {
//                             phrasePart.UpdateEntityName(existingEntityName);
//                         }
//                         else
//                         {
//                             var newEntityName = new EntityName(phrasePart.EntityName.Name,
//                                 query.ProjectId, true);
//                             phrasePart.UpdateEntityName(newEntityName);
//                             entityNamesToCreate.Add(newEntityName);
//                         }
//                     }
//
//                     if (phrasePart.EntityTypeId.HasValue && !entityTypeIds.Contains(phrasePart.EntityTypeId.Value))
//                     {
//                         throw new BadRequestException(ErrorDescriptions.EntityTypeNotFound);
//                     }
//                 }
//
//                 foreach (var newEntityName in entityNamesToCreate)
//                 {
//                     await _entityNameRepository.Add(newEntityName);
//                 }
//
//                 if (existingIntent != null)
//                 {
//                     existingIntent.UpdateName(request.Intent.Name);
//                     existingIntent.UpdatePhrases(request.Intent.PhraseParts);
//                 }
//                 else
//                 {
//                     query.ClearIntents();
//                     query.AddIntent(new Intent(request.Intent.Name, query.ProjectId, request.Intent.Type,
//                         request.Intent.PhraseParts));
//                 }
//             }
//             else
//             {
//                 throw new BadRequestException("Missing intent and intentId, one of them should be specified.");
//             }
//             
//             if (request.ResponseId.HasValue)
//             {
//                 var response = await _responseRepository.GetResponseById(request.ResponseId.Value);
//                 if (response == null) throw new BadRequestException(ResponseNotFound);
//                 query.ClearResponses();
//                 query.AddResponse(response);
//             }
//             else if (request.Responses?.Any() == true)
//             {
//                 var entityNames = await _entityNameRepository.ListByProjectId(query.ProjectId);
//                 for (var i = 0; i < request.Responses.Length; i++)
//                 {
//                     if (request.RteText[i] != null)
//                     {
//                         request.Responses[i].SetRteText(request.RteText[i]!,
//                             entityNames.ToDictionary(e => e.Name));
//                     }
//                 }
//
//                 // TODO: this won't delete the detached response though
//                 query.ClearResponses();
//                 foreach (var response in request.Responses)
//                 {
//                     query.AddResponse(new Response(response.Resolution,
//                         query.ProjectId, response.Type, response.Order));
//                 }
//             }
//             else
//             {
//                 throw new BadRequestException("Missing response and responseId, one of them should be specified.");
//             }
//             
//             await _unitOfWork.SaveChanges();
//             return query;
//         }
//     }
// }