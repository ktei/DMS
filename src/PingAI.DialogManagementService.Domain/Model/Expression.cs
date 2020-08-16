using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Expression : ValueObject
    {
        [JsonPropertyName("entityName")]
        public string EntityName { get; set; }
        
        [JsonPropertyName("condition")]
        public Condition Condition { get; set; }

        public override string ToString() => $"{EntityName} {Condition}";

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return EntityName;
            yield return Condition;
        }
    }

    public class Condition : ValueObject
    {
        [JsonPropertyName("operator")]
        public Operator Operator { get; set; }
        
        [JsonPropertyName("value")]
        public string? Value { get; set; }
        
        [JsonPropertyName("entityNameId")]
        public Guid? EntityNameId { get; set; }
        
        [JsonPropertyName("comparer")]
        public Comparer Comparer { get; set; }

        public Condition(Operator @operator, string? value, Guid? entityNameId, Comparer comparer)
        {
            Operator = @operator;
            Value = value;
            EntityNameId = entityNameId;
            Comparer = comparer;
        }
        
        public static Condition ValueCondition(Operator @operator, string value) =>
            new Condition(@operator, value, null, Comparer.VALUE);
        public static  Condition EntityCondition(Operator @operator, Guid entityNameId) =>
            new Condition(@operator, null, entityNameId, Comparer.OTHER_ENTITY_NAME);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Operator;
            yield return Value;
            yield return EntityNameId;
            yield return Comparer;
        }

        public override string ToString() => Comparer switch
        {
            Comparer.VALUE => $"{Operator} {Value}",
            Comparer.OTHER_ENTITY_NAME => $"{Operator} {EntityNameId}",
            _ => base.ToString()
        };
    }

    public enum Operator
    {
        CONTAINS,
        ENDSWITH,
        EQ,
        GT,
        GTE,
        KNOWN,
        LT,
        LTE,
        NE,
        STARTSWITH,
        UNKNOWN
    }

    public enum Comparer
    {
        VALUE,
        OTHER_ENTITY_NAME
    }
}

