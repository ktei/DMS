using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Queries.Shared
{
    internal static class ResponseHelper
    {
        public static async Task<Domain.Model.Response[]> CreateResponses(Guid projectId,
            IEnumerable<Response> responses,
            IReadOnlyList<EntityName> entityNames,
            IResponseRepository responseRepository)
        {
            var results = new List<Domain.Model.Response>();
            foreach (var resp in responses)
            {
                if (resp.RteText != null)
                {
                    var resolution = Resolution.Factory.RteText(resp.RteText,
                        entityNames.ToDictionary(x => x.Name));
                    results.Add(new Domain.Model.Response(projectId, resolution, resp.type,
                        resp.Order));
                }
                else if (resp.Form != null)
                {
                    var resolution = Resolution.Factory.Form(resp.Form);
                    results.Add(new Domain.Model.Response(projectId, resolution, ResponseType.FORM,
                        resp.Order));
                }
                else if (resp.WebhookResponseId != null)
                {
                    var webhookResponse = await responseRepository.FindById(resp.WebhookResponseId.Value);
                    if (webhookResponse == null)
                        throw new BadRequestException($"Could not find webhook response {resp.WebhookResponseId}");
                    results.Add(webhookResponse);
                }
            }

            return results.ToArray();
        }
    }
}
