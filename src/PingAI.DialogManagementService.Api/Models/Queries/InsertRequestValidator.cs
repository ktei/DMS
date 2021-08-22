using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
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