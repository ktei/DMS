using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Ver : ValueObject
    {
        public const int DesignTime = 0;
        
        public int Version { get; }

        public Ver(int version)
        {
            if (version < DesignTime)
                throw new ArgumentException(nameof(version));
            Version = version;
        }

        public bool IsDesignTime => Version == 0;
        
        public static Ver NewDesignTime() => new Ver(DesignTime);

        public Ver Next() => new Ver(Version + 1);
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Version;
        }

        public override string ToString() => $"v{Version}";

        public static implicit operator int(Ver v) => v.Version;
        public static explicit operator Ver(int v) => new Ver(v);
    }
}