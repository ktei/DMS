using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class ConditionDto
    {
        public string Operator { get; set; }
        public string? Value { get; set; }
        public string? EntityNameId { get; set; }
        public string Comparer { get; set; }

        public ConditionDto(string @operator, string? value, string? entityNameId, string comparer)
        {
            Operator = @operator;
            Value = value;
            EntityNameId = entityNameId;
            Comparer = comparer;
        }

        public ConditionDto(Condition condition) : this(condition.Operator.ToString(),
            condition.Value, condition.EntityNameId?.ToString(), condition.Comparer.ToString())
        {
            
        }

        public ConditionDto()
        {
            
        }
    }
}