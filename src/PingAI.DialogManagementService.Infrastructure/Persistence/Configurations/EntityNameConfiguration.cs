using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class EntityNameConfiguration :  IEntityTypeConfiguration<EntityName>
    {
        public void Configure(EntityTypeBuilder<EntityName> builder)
        {
            builder.ToTable("EntityNames", "chatbot");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.CanBeReferenced)
                .HasColumnName("canBeReferenced");

            builder.AttachTimestamps();
        }
    }
}