using System.Reflection;
using FluentValidation;
using Newtonsoft.Json;

namespace PingAI.DialogManagementService.Api.Validations
{
    internal static class RuleBuilderOptionsExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> UseJsonPathInErrorMessage<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.Configure(cfg =>
            {

                var jsonAttribute = cfg.Member?.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsonAttribute != null)
                {
                    cfg.PropertyName = jsonAttribute.PropertyName;
                }

                cfg.MessageBuilder = context =>
                {
                    context.MessageFormatter
                        .AppendPropertyName(context
                            .PropertyName); // Default behaviour uses context.DisplayName, by changing it to context.PropertyName this uses the full path.
                    return context.GetDefaultMessage();
                };

            });
        }
    }
}