using System.Collections.Generic;
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
        private readonly IEntityNameRepository _entityNameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public CreateQueryCommandHandler(IQueryRepository queryRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            IEntityNameRepository entityNameRepository)
        {
            _queryRepository = queryRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _entityNameRepository = entityNameRepository;
        }

        public async Task<Query> Handle(CreateQueryCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ErrorDescriptions.ProjectWriteDenied);

            var displayOrder = (await _queryRepository.GetMaxDisplayOrder(request.ProjectId)) + 1;
            var query = new Query(request.ProjectId, request.Name, request.Expressions,
                request.Description, request.Tags, displayOrder);

            var entityNames = await _entityNameRepository.ListByProjectId(request.ProjectId);

            var intent = CreateIntent(request, entityNames);

            query.AddIntent(intent);

            foreach (var response in CreateResponses(request, entityNames))
            {
                query.AddResponse(response);
            }

            await _queryRepository.Add(query);
            await _unitOfWork.SaveChanges();
            return query;
        }

        private static Intent CreateIntent(CreateQueryCommand request, IReadOnlyList<EntityName> entityNames)
        {
            var intent = new Intent(request.ProjectId, request.Name, IntentType.STANDARD);
            var phraseDisplayOrder = 0;
            foreach (var groupedPhraseParts in request.PhraseParts.GroupBy(p => p.PhraseId))
            {
                var phrase = new Phrase(phraseDisplayOrder++);
                foreach (var phrasePart in groupedPhraseParts)
                {
                    if (phrasePart.EntityName != null)
                    {
                        var existingEntityName = entityNames
                            .FirstOrDefault(e => e.Name == phrasePart.EntityName);
                        phrase.AppendEntity(phrasePart.Text,
                            existingEntityName ?? new EntityName(phrasePart.EntityName, true));
                    }
                    else
                    {
                        phrase.AppendText(phrasePart.Text);
                    }
                }

                intent.AddPhrase(phrase);
            }

            return intent;
        }

        private IEnumerable<Domain.Model.Response> CreateResponses(CreateQueryCommand request,
            IReadOnlyList<EntityName> entityNames)
        {
            foreach (var resp in request.Responses)
            {
                if (resp.RteText != null)
                {
                    var resolution = Resolution.Factory.RteText(resp.RteText,
                        entityNames.ToDictionary(x => x.Name));
                    yield return new Domain.Model.Response(request.ProjectId, resolution, ResponseType.RTE,
                        resp.Order);
                }
                else if (resp.Form != null)
                {
                    var resolution = Resolution.Factory.Form(resp.Form);
                    yield return new Domain.Model.Response(request.ProjectId, resolution, ResponseType.FORM,
                        resp.Order);
                }
                // TODO: webhook
            }
        }
    }
}
