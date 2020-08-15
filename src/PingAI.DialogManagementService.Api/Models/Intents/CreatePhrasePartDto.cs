using System;
using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class CreatePhrasePartDto
    {
        public int? Position { get; set; }
        public string? Text { get; set; }
        public string? Value { get; set; }
        public string Type { get; set; }
        public string? EntityName { get; set; }
        public string? EntityTypeId { get; set; }
    }

    public class CreatePhrasePartDtoValidator : AbstractValidator<CreatePhrasePartDto>
    {
        public CreatePhrasePartDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .MustBeEnum(typeof(PhrasePartType));
            
            // For TEXT
            RuleFor(x => x.Text)
                .NotNull()
                .When(x =>
                    string.Compare(x.Type, PhrasePartType.TEXT.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .WithMessage("{PropertyName} must not be null given TEXT type");
           
            // For ENTITY
            RuleFor(x => x.Text)
                .NotNull()
                .When(x => 
                    string.Compare(x.Type, PhrasePartType.ENTITY.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .WithMessage("{PropertyName} must not be null given ENTITY type");
            RuleFor(x => x.EntityName)
                .NotNull()
                .MaximumLength(30) // 30 is the parameter name length limit of Dialogflow
                .When(x => 
                    string.Compare(x.Type, PhrasePartType.ENTITY.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .WithMessage("{PropertyName} must not be null given ENTITY type");
            RuleFor(x => x.EntityTypeId)
                .NotNull()
                .When(x => 
                    string.Compare(x.Type, PhrasePartType.ENTITY.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .WithMessage("{PropertyName} must not be null given ENTITY type");
            
            // For CONSTANT_ENTITY
            RuleFor(x => x.Value)
                .NotEmpty()
                .When(x =>
                    string.Compare(x.Type, PhrasePartType.CONSTANT_ENTITY.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .WithMessage("{PropertyName} must not be empty given CONSTANT_ENTITY type");
            RuleFor(x => x.EntityName)
                .NotNull()
                .MaximumLength(30) // 30 is the parameter name length limit of Dialogflow
                .When(x => 
                    string.Compare(x.Type, PhrasePartType.CONSTANT_ENTITY.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .WithMessage("{PropertyName} must not be null given CONSTANT_ENTITY type");
            RuleFor(x => x.EntityTypeId)
                .NotNull()
                .When(x => 
                    string.Compare(x.Type, PhrasePartType.CONSTANT_ENTITY.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .WithMessage("{PropertyName} must not be null given CONSTANT_ENTITY type");
        }
    }
}