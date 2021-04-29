using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Utils;
using static PingAI.DialogManagementService.Domain.Model.ResolutionPart;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Response : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string? ResolutionJson { get; private set; }

        public Resolution? Resolution =>
            ResolutionJson == null ? null : JsonUtils.TryDeserialize<Resolution>(ResolutionJson);
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public ResponseType Type { get; private set; }
        public int Order { get; private set; }
        public string? SpeechContexts { get; private set; }

        private readonly List<QueryResponse> _queryResponses;
        public IReadOnlyList<QueryResponse> QueryResponses => _queryResponses.ToImmutableList();

        public const int MaxRteTextLength = 4000;
        public const int QuickReplyLength = 250;
        
        public Response(Resolution? resolution, Guid projectId, ResponseType type,
            int order) : this(JsonUtils.Serialize(resolution ?? Resolution.Empty),
            projectId, type, order, null)
        {
        }

        public Response(string? resolutionJson, Guid projectId, ResponseType type,
            int order, string? speechContexts)
        {
            ResolutionJson = resolutionJson;
            ProjectId = projectId;
            Type = type;
            Order = order;
            SpeechContexts = speechContexts;
            _queryResponses = new List<QueryResponse>();
        }
        
        public Response(Guid projectId, ResponseType type,
            int order) : this(Resolution.Empty, projectId, type, order)
        {
        }

        public Response(ResponseType type, int order)
        {
            Type = type;
            Order = order;
            ResolutionJson = JsonUtils.Serialize(Resolution.Empty);
            _queryResponses = new List<QueryResponse>();
        }

        /// <summary>
        /// Sets response's type as RTE and parses the raw text
        /// into an array of <see cref="ResolutionPart"/>
        /// </summary>
        /// <param name="rteText">RTE text that will be parsed to array of <see cref="ResolutionPart"/></param>
        /// <param name="entityNames">A map of all entity names of current project, the key being the name</param>
        public void SetRteText(string rteText, IDictionary<string, EntityName> entityNames)
        {
            if (string.IsNullOrEmpty(rteText))
                throw new ArgumentException($"{nameof(rteText)} cannot be empty");
            
            if (rteText.Length > MaxRteTextLength)
                throw new ArgumentException($"Max length of {nameof(rteText)} is {MaxRteTextLength}");
                
            var resolutionParts = new List<ResolutionPart>();
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
                    resolutionParts.Add(TextPart(rteText.Substring(pos, m.Index - pos)));
                }
                
                // add current param
                if (entityNames.TryGetValue(m.Groups[1].Value, out var entityName))
                {
                    resolutionParts.Add(EntityNamePart(entityName.Id));
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
                resolutionParts.Add(TextPart(rteText.Substring(pos)));
            }

            ResolutionJson = JsonUtils.Serialize(new Resolution(resolutionParts.ToArray()));
        }

        public void SetForm(FormResolution form)
        {
            ResolutionJson = JsonUtils.Serialize(new Resolution(form ?? throw new ArgumentNullException(nameof(form))));
            Type = ResponseType.FORM;
        }

        public string GetDisplayText()
        {
            var sb = new StringBuilder();
            if (Resolution == null)
                return string.Empty;
            foreach (var resolutionPart in Resolution.Parts ?? new ResolutionPart[0])
            {
                sb.Append(resolutionPart.Text);
            }

            return sb.ToString();
        }
    }
    
    public enum ResponseType
    {
        RTE,
        HANDOVER,
        VIDEO,
        SOCIAL,
        WEBHOOk,
        FORM,
        QUICK_REPLY
    }
}

