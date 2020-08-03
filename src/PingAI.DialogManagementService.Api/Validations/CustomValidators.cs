using System;
using FluentValidation;

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
            }).WithMessage("{PropertyName} must be a valid UUID/GUID");
        }
    }
}