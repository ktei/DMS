using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public abstract class EntityConfigurationBase<T> : IEntityTypeConfiguration<T> where T : class
    {
        void IEntityTypeConfiguration<T>.Configure(EntityTypeBuilder<T> builder)
        {
            builder.AddId();
            Configure(builder);
            builder.AddTimestamps();
        }

        protected abstract void Configure(EntityTypeBuilder<T> builder);
    }
}
