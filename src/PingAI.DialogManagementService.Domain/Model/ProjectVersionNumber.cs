using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ProjectVersionNumber : ValueObject
    {
        public const int DesignTime = 0;
        
        public int Version { get; }

        public ProjectVersionNumber(int version)
        {
            if (version < DesignTime)
                throw new ArgumentException(nameof(version));
            Version = version;
        }

        public bool IsDesignTime => Version == 0;
        
        public static ProjectVersionNumber NewDesignTime() => new ProjectVersionNumber(DesignTime);

        public ProjectVersionNumber Next() => new ProjectVersionNumber(Version + 1);
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Version;
        }

        public override string ToString() => $"v{Version}";

        public static implicit operator int(ProjectVersionNumber v) => v.Version;
        public static explicit operator ProjectVersionNumber(int v) => new ProjectVersionNumber(v);
    }
}