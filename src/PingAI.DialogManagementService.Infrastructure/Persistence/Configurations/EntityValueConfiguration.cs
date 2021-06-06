using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class EntityValueConfiguration : EntityConfigurationBase<EntityValue>
    {
        protected override void Configure(EntityTypeBuilder<EntityValue> builder)
        {
            builder.ToTable("EntityValues", "chatbot");
            builder.Property(o => o.Value)
                .HasColumnName("value")
                .HasMaxLength(255);
            builder.Property(o => o.EntityTypeId)
                .HasColumnName("entityTypeId");
            builder.Property(o => o.Synonyms)
                .HasColumnName("synonyms");
        }
    }
}