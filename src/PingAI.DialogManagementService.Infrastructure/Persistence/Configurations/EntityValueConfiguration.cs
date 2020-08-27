using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class EntityValueConfiguration : IEntityTypeConfiguration<EntityValue>
    {
        public void Configure(EntityTypeBuilder<EntityValue> builder)
        {
            builder.ToTable("EntityValues", "chatbot");
            builder.ConfigureId();

            builder.Property(o => o.Value)
                .HasColumnName("value")
                .HasMaxLength(255);
            builder.Property(o => o.EntityTypeId)
                .HasColumnName("entityTypeId");
            builder.Property(o => o.Synonyms)
                .HasColumnName("synonyms");
            
            builder.AttachTimestamps();        
        }
    }
}