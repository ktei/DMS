using FluentValidation;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UpdateEnquiriesRequestValidator : AbstractValidator<UpdateEnquiriesRequest>
    {
        public UpdateEnquiriesRequestValidator()
        {
            RuleForEach(x => x.Enquiries)
                .NotEmpty();
        }
    }
}