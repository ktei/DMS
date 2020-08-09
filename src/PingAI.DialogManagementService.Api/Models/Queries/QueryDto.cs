using System;
using System.Linq;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Api.Models.Responses;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public ExpressionDto[] Expressions { get; private set; }
        public string Description { get; private set; }
        public string[]? Tags { get; private set; }
        public int DisplayOrder { get; private set; }
        public IntentDto[] Intents { get; set; }
        public ResponseDto[] Responses { get; set; }


        public QueryDto(string id, string name, string projectId, ExpressionDto[] expressions, string description,
            string[]? tags, int displayOrder, IntentDto[] intents, ResponseDto[] responses)
        {
            Id = id;
            Name = name;
            ProjectId = projectId;
            Expressions = expressions;
            Description = description;
            Tags = tags;
            DisplayOrder = displayOrder;
            Intents = intents;
            Responses = responses;
        }

        public QueryDto(Query query) : this(query.Id.ToString(), query.Name, query.ProjectId.ToString(),
            query.Expressions.Select(e => new ExpressionDto(e)).ToArray(),query.Description,
            query.Tags, query.DisplayOrder,
            query.QueryIntents.Select(i => 
                new IntentDto(i.Intent ?? 
                              throw new InvalidOperationException($"{nameof(i.Intent)} was not loaded"))).ToArray(),
            query.QueryResponses.Select(r =>
                new ResponseDto(r.Response ?? 
                                throw new InvalidOperationException($"{nameof(r.Response)} was not loaded"))).ToArray())
        {
            
        }
    }
}