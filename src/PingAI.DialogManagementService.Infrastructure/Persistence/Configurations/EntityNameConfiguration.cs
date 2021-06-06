using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class EntityNameConfiguration :  EntityConfigurationBase<EntityName>
    {
        protected override void Configure(EntityTypeBuilder<EntityName> builder)
        {
            builder.ToTable("EntityNames", "chatbot");
            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.CanBeReferenced)
                .HasColumnName("canBeReferenced");
        }
    }
}