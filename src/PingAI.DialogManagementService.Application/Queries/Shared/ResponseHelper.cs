using System;
using System.Collections.Generic;
using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.Shared
{
    internal static class ResponseHelper
    {
        public static IEnumerable<Domain.Model.Response> CreateResponses(Guid projectId,
            IEnumerable<Response> responses,
            IReadOnlyList<EntityName> entityNames)
        {
            foreach (var resp in responses)
            {
                if (resp.RteText != null)
                {
                    var resolution = Resolution.Factory.RteText(resp.RteText,
                        entityNames.ToDictionary(x => x.Name));
                    yield return new Domain.Model.Response(projectId, resolution, ResponseType.RTE,
                        resp.Order);
                }
                else if (resp.Form != null)
                {
                    var resolution = Resolution.Factory.Form(resp.Form);
                    yield return new Domain.Model.Response(projectId, resolution, ResponseType.FORM,
                        resp.Order);
                }
                // TODO: webhook
            }
        }
    }
}
