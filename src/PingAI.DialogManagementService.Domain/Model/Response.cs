using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PingAI.DialogManagementService.Domain.Utils;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Response : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public Resolution Resolution { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public ResponseType Type { get; private set; }
        public int Order { get; private set; }
        public string? SpeechContexts { get; private set; }
        private List<Query> _queries;
        public IReadOnlyList<Query> Queries => _queries.ToImmutableList();

        public const int MaxRteTextLength = 4000;
        public const int QuickReplyLength = 250;
        
        public Response(Guid projectId, Resolution resolution, ResponseType type,
            int order) : this(resolution, type, order)
        {
            if (projectId.IsEmpty())
                throw new ArgumentException($"{nameof(projectId)} cannot be empty.");
            ProjectId = projectId;
        }

        public Response(Resolution resolution, ResponseType type,
            int order)
        {
            Resolution = resolution ?? throw new ArgumentNullException(nameof(resolution));
            Type = type;
            Order = order;
            SpeechContexts = null;
            _queries = new List<Query>();
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
}
