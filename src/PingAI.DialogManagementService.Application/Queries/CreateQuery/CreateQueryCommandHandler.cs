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

            var queryDisplayOrder = (await _queryRepository.GetMaxDisplayOrder(request.ProjectId)) + 1;
            var query = new Query(request.Name, request.ProjectId, request.Expressions,
                request.Description, request.Tags, queryDisplayOrder);

            var intent = new Intent(request.Name, IntentType.STANDARD);
            var entityNames = await _entityNameRepository.ListByProjectId(request.ProjectId);

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
            query.AddIntent(intent);

            foreach (var resp in request.Responses)
            {
                if (resp.RteText != null)
                {
                    var resolution = Resolution.Factory.RteText(resp.RteText,
                        entityNames.ToDictionary(x => x.Name));
                    query.AddResponse(new Domain.Model.Response(resolution, ResponseType.RTE,
                        resp.Order));
                }
                else if (resp.Form != null)
                {
                    var resolution = Resolution.Factory.Form(resp.Form);
                    query.AddResponse(new Domain.Model.Response(resolution, ResponseType.FORM,
                        resp.Order));
                }
            }

            query = await _queryRepository.AddQuery(query);
            await _unitOfWork.SaveChanges();
            return query;
        }
    }
}
