using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ProjectVersionNumber : ValueObject
    {
        public const int DesignTime = 0;
        
        public int Number { get; }

        public ProjectVersionNumber(int number)
        {
            if (number < DesignTime)
                throw new ArgumentException(nameof(number));
            Number = number;
        }

        public bool IsDesignTime => Number == 0;
        
        public static ProjectVersionNumber NewDesignTime() => new ProjectVersionNumber(DesignTime);

        public ProjectVersionNumber Next() => new ProjectVersionNumber(Number + 1);
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Number;
        }

        public override string ToString() => $"v{Number}";

        public static implicit operator int(ProjectVersionNumber v) => v.Number;
        public static explicit operator ProjectVersionNumber(int v) => new ProjectVersionNumber(v);
    }
}