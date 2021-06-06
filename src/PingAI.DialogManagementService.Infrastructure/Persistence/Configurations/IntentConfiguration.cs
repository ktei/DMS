using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class IntentConfiguration : EntityConfigurationBase<Intent>
    {
        protected override void Configure(EntityTypeBuilder<Intent> builder)
        {
            builder.ToTable("Intents", "chatbot");

            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder
                .Property<string>("_iconName")
                .HasField("_iconName")
                .HasColumnName("iconName");
            builder
                .Property<string>("_color")
                .HasField("_color")
                .HasColumnName("color");
            builder.Property(o => o.Type)
                .HasColumnName("type");

            builder.HasMany(o => o.PhraseParts)
                .WithOne(p => p.Intent);
        }
    }
}