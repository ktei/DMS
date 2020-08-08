using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Expression : ValueObject
    {
        public string EntityName { get; private set; }
        public Condition Condition { get; private set; }

        public override string ToString() => $"{EntityName} {Condition}";

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return EntityName;
            yield return Condition;
        }
    }

    public class Condition : ValueObject
    {
        public Operator Operator { get; private set; }
        public string? Value { get; private set; }
        public Guid? EntityNameId { get; private set; }
        public Comparer Comparer { get; private set; }

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

