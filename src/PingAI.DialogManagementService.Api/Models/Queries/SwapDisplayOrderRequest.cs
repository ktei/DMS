using System;
using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class SwapDisplayOrderRequest
    {
        public string QueryId { get; set; }
        public string TargetQueryId { get; set; }

        public SwapDisplayOrderRequest(string queryId, string targetQueryId)
        {
            QueryId = queryId;
            TargetQueryId = targetQueryId;
        }

        public SwapDisplayOrderRequest()
        {
            
        }
    }

    public class SwapDisplayOrderRequestValidator : AbstractValidator<SwapDisplayOrderRequest>
    {
        public SwapDisplayOrderRequestValidator()
        {
            RuleFor(x => x.QueryId)
                .NotEmpty()
                .MustBeGuid();
            RuleFor(x => x.TargetQueryId)
                .NotEmpty()
                .MustBeGuid();
        }
    }
}