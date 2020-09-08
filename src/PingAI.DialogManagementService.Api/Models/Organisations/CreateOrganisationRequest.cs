using System;
using FluentValidation;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Organisations
{
    public class CreateOrganisationRequest
    {
        public Guid? AdminUserId { get; set; }
        public string Name { get; set; }
    }

    public class CreateOrganisationRequestValidator : AbstractValidator<CreateOrganisationRequest>
    {
        public CreateOrganisationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Organisation.MaxNameLength);
        }
    }
}