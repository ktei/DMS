using FluentValidation;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Users
{
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Auth0Id { get; set; }
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(User.MaxNameLength);
            RuleFor(x => x.Auth0Id)
                .NotEmpty();
        }
    }
}