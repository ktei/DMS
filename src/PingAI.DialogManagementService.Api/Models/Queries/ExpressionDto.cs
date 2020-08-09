using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class ExpressionDto
    {
        public string EntityName { get; set; }
        public ConditionDto Condition { get; set; }


        public ExpressionDto(string entityName, ConditionDto condition)
        {
            EntityName = entityName;
            Condition = condition;
        }

        public ExpressionDto(Expression expression) : this(expression.EntityName, new ConditionDto(expression.Condition))
        {
            
        }

        public ExpressionDto()
        {
            
        }
    }
}