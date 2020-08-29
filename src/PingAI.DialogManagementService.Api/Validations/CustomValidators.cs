using System;
using FluentValidation;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Validations
{
    internal static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> MustBeGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(v =>
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        return true;
                    }

                    if (Guid.TryParse(v, out _))
                        return true;

                    return false;
                })
                .UseJsonPathInErrorMessage()
                .WithMessage("'{PropertyName}' must be a valid UUID/GUID");
        }

        public static IRuleBuilderOptions<T, string> MustBeEnum<T>(this IRuleBuilder<T, string> ruleBuilder,
            Type enumType)
        {
            return ruleBuilder.Must(v =>
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        return true;
                    }

                    if (Enum.TryParse(enumType, v, true, out _))
                        return true;

                    return false;
                })
                .UseJsonPathInErrorMessage()
                .WithMessage($"'{{PropertyName}}' must be a valid enum of {enumType.Name}: [" +
                             $"{string.Join(", ", Enum.GetNames(enumType))}]");
        }

        public static IRuleBuilderOptions<T, string> MustBeUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(v =>
                string.IsNullOrEmpty(v) || 
                Uri.TryCreate(v, UriKind.Absolute, out var uriResult) && 
                (uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeHttp))
                .WithMessage("'{PropertyName}' must be a valid URL");
        }
    }
}