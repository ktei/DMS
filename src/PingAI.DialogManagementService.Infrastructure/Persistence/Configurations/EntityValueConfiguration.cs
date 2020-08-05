using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class EntityValueConfiguration : IEntityTypeConfiguration<EntityValue>
    {
        public void Configure(EntityTypeBuilder<EntityValue> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.Value)
                .HasColumnName("value")
                .HasMaxLength(255);
            builder.Property(o => o.EntityTypeId)
                .HasColumnName("entityTypeId");
            builder.Property(o => o.Synonyms)
                .HasColumnName("synonyms");
            
            builder.Property(o => o.CreatedAt)
                .HasColumnName("createdAt");
            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updatedAt");
        }
    }
}