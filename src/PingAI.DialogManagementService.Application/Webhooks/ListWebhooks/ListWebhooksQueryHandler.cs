using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Webhooks.ListWebhooks
{
    public class ListWebhooksQueryHandler : IRequestHandler<ListWebhooksQuery, IReadOnlyList<Response>>
    {
        private readonly IResponseRepository _responseRepository;

        public ListWebhooksQueryHandler(IResponseRepository responseRepository)
        {
            _responseRepository = responseRepository;
        }

        public async Task<IReadOnlyList<Response>> Handle(ListWebhooksQuery request,
            CancellationToken cancellationToken)
        {
            var responses = await _responseRepository.ListByProjectId(request.ProjectId, ResponseType.WEBHOOK);
            return responses;
        }
    }
}