using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.OrganisationId)
                .HasColumnName("organisationId");
            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.FallbackMessage)
                .HasColumnName("fallbackMessage");
            builder.Property(o => o.GreetingMessage)
                .HasColumnName("greetingMessage");
            builder.Property(o => o.WidgetColor)
                .HasColumnName("widgetColor");
            builder.Property(o => o.WidgetDescription)
                .HasColumnName("widgetDescription");
            builder.Property(o => o.WidgetTitle)
                .HasColumnName("widgetTitle");
            builder.Property(o => o.Enquiries)
                .HasColumnName("enquiries");

            builder.Property(o => o.CreatedAt)
                .HasColumnName("createdAt");
            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updatedAt");
            
            builder.HasMany(o => o.Intents)
                .WithOne(p => p.Project);
            builder.HasMany(o => o.EntityTypes)
                .WithOne(e => e.Project);
            builder.HasMany(o => o.EntityNames)
                .WithOne(e => e.Project);
            builder.HasMany(o => o.Responses)
                .WithOne(o => o.Project);
        }
    }
}