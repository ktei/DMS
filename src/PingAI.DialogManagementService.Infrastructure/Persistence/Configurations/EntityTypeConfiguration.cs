using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class EntityTypeConfiguration : EntityConfigurationBase<EntityType>
    {
        protected override void Configure(EntityTypeBuilder<EntityType> builder)
        {
            builder.ToTable("EntityTypes", "chatbot");
            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.Description)
                .HasColumnName("description");
            builder.Property(o => o.Tags)
                .HasColumnName("tags");
            
            builder.HasMany(o => o.Values)
                .WithOne(v => v.EntityType);
        }
    }
}