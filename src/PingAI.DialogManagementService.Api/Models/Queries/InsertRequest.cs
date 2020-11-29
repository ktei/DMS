using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class InsertRequest
    {
        public string QueryId { get; set; }
        public int DisplayOrder{ get; set; }

        public InsertRequest(string queryId, int displayOrder)
        {
            QueryId = queryId;
            DisplayOrder = displayOrder;
        }

        public InsertRequest()
        {
            
        }
    }
    
    public class InsertRequestValidator : AbstractValidator<InsertRequest>
    {
        public InsertRequestValidator()
        {
            RuleFor(x => x.QueryId)
                .NotEmpty()
                .MustBeGuid();
            RuleFor(x => x.DisplayOrder)
                .Must(x => x >= 0);
        }
    }
}