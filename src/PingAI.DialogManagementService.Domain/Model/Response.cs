using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using static PingAI.DialogManagementService.Domain.Model.ResolutionPart;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Response : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public ResolutionPart[] Resolution { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public ResponseType Type { get; private set; }
        public int Order { get; private set; }
        
        private readonly List<QueryResponse> _queryResponses = new List<QueryResponse>();
        public IReadOnlyList<QueryResponse> QueryResponses => _queryResponses.ToImmutableList();
        
        public Response(ResolutionPart[] resolution, Guid projectId, ResponseType type,
            int order)
        {
            Resolution = (resolution ?? new ResolutionPart[0]).ToArray();
            ProjectId = projectId;
            Type = type;
            Order = order;
        }
        
        /// <summary>
        /// Sets response's type as RTE and parses the raw text
        /// into an array of <see cref="ResolutionPart"/>
        /// </summary>
        /// <param name="rteText">RTE text that will be parsed to array of <see cref="ResolutionPart"/></param>
        /// <param name="entityNames">A map of all entity names of current project, the key being the name</param>
        public void SetRteText(string rteText, IDictionary<string, EntityName> entityNames)
        {
            var resolution = new List<ResolutionPart>();
            var re = new Regex(@"\$\{([A-Za-z-_0-9]+)\}");
            var pos = 0;
            Match m;
            do
            {
                m = re.Match(rteText, pos);
                if (!m.Success) break;
                
                if (m.Index > pos)
                {
                    // add plain text before current param
                    resolution.Add(TextPart(rteText.Substring(pos, m.Index - pos)));
                }
                
                // add current param
                if (entityNames.TryGetValue(m.Groups[1].Value, out var entityName))
                {
                    resolution.Add(EntityNamePart(entityName.Id));
                }
                else
                {
                    throw new BadRequestException(
                        string.Format(ErrorDescriptions.EntityNameNotFound, m.Groups[1].Value));
                }
                
                // move the cursor to the end of the current param
                pos = m.Index + m.Groups[0].Length;
            } while (m!.Success);
            
            // add the rest of the plain text
            if (rteText.Length > pos) {
                resolution.Add(TextPart(rteText.Substring(pos)));
            }

            Resolution = resolution.ToArray();
        }
    }
    
    public enum ResponseType
    {
        RTE,
        FACEBOOK,
        VIDEO
    }
}