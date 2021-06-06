using System;
using System.Collections.Generic;
using System.Linq;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Api.Models.Responses;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryDto
    {
        public string QueryId { get; set; }
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public ExpressionDto[] Expressions { get; set; }
        public string Description { get; set; }
        public string[]? Tags { get; set; }
        public int DisplayOrder { get; set; }
        public IntentDto[] Intents { get; set; }
        public ResponseDto[] Responses { get; set; }


        public QueryDto(string queryId, string name, string projectId, 
            IEnumerable<ExpressionDto> expressions, string description,
            IEnumerable<string>? tags, int displayOrder, 
            IEnumerable<IntentDto> intents, 
            IEnumerable<ResponseDto> responses)
        {
            QueryId = queryId;
            Name = name;
            ProjectId = projectId;
            Expressions = expressions.ToArray();
            Description = description;
            Tags = tags?.ToArray();
            DisplayOrder = displayOrder;
            Intents = intents.ToArray();
            Responses = responses.ToArray();
        }

        public QueryDto(Query query)
        {
            QueryId = query.Id.ToString();
            Name = query.Name;
            ProjectId = query.ProjectId.ToString();
            Expressions = query.Expressions.Select(e => new ExpressionDto(e)).ToArray();
            Intents = query.Intents.Select(i => new IntentDto(i)).ToArray();
            Responses = query.Responses.Select(r => new ResponseDto(r)).ToArray();
            Description = query.Description;
            Tags = query.Tags?.ToArray();
        }
    }
}
