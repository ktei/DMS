using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class PhrasePartConfiguration : EntityConfigurationBase<PhrasePart>
    {
        protected override void Configure(EntityTypeBuilder<PhrasePart> builder)
        {
            builder.ToTable("PhraseParts", "chatbot");

            builder.Property(o => o.Position)
                .HasColumnName("position");
            builder.Property(o => o.Text)
                .HasColumnName("text");

            builder.Property(o => o.Type)
                .HasColumnName("type");

            builder.Property(o => o.Value)
                .HasColumnName("value");

            builder.Property(o => o.IntentId)
                .HasColumnName("intentId");

            builder.Property(o => o.PhraseId)
                .HasColumnName("phraseId");

            builder.Property(o => o.EntityNameId)
                .HasColumnName("entityNameId");

            builder.Property(o => o.EntityTypeId)
                .HasColumnName("entityTypeId");
            
            builder.Property(o => o.DisplayOrder)
                .HasColumnName("displayOrder");

            builder.HasOne(o => o.EntityName);
            builder.HasOne(o => o.EntityType);
        }
    }
}