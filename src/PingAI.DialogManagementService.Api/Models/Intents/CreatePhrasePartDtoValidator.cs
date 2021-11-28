using System;
using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class CreatePhrasePartDtoValidator : AbstractValidator<CreatePhrasePartDto>
    {
        public CreatePhrasePartDtoValidator()
        {
            RuleFor(x => x.Text)
                .MaximumLength(PhrasePart.MaxTextLength);
            RuleFor(x => x.Value)
                .MaximumLength(PhrasePart.MaxValueLength);
            
            // For TEXT
            RuleFor(x => x.Text)
                .NotNull()
                .When(x =>
                    x.Type == PhrasePartType.TEXT)
                .WithMessage("{PropertyName} must not be null given TEXT type");
           
            // For ENTITY
            RuleFor(x => x.Text)
                .NotEmpty()
                .When(x =>
                    x.Type == PhrasePartType.ENTITY)
                .WithMessage("{PropertyName} must not be null given ENTITY type");
            RuleFor(x => x.EntityName)
                .NotNull()
                .MaximumLength(EntityName.MaxNameLength)
                .When(x => 
                    x.Type == PhrasePartType.ENTITY)
                .WithMessage("{PropertyName} must not be null given ENTITY type");
            
            // For CONSTANT_ENTITY
            RuleFor(x => x.Value)
                .NotEmpty()
                .When(x =>
                    x.Type == PhrasePartType.CONSTANT_ENTITY)
                .WithMessage("{PropertyName} must not be empty given CONSTANT_ENTITY type");
            RuleFor(x => x.EntityName)
                .NotNull()
                .MaximumLength(EntityName.MaxNameLength)
                .When(x => 
                    x.Type == PhrasePartType.CONSTANT_ENTITY)
                .WithMessage("{PropertyName} must not be null given CONSTANT_ENTITY type");
        }
    }
}