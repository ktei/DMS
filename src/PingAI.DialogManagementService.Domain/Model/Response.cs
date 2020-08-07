using System;
using System.Collections.Generic;
using System.Linq;

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
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        

        public Response(Guid id, ResolutionPart[] resolution, Guid projectId, ResponseType type,
            int order)
        {
            Id = id;
            Resolution = (resolution ?? new ResolutionPart[0]).ToArray();
            ProjectId = projectId;
            Type = type;
            Order = order;
        }
    }
    
    public enum ResponseType
    {
        RTE,
        FACEBOOK,
        VIDEO
    }
}