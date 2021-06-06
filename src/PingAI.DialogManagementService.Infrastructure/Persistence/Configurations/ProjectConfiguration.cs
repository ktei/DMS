using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class ProjectConfiguration : EntityConfigurationBase<Project>
    {
        protected override void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects", "chatbot");

            builder.Property(o => o.OrganisationId)
                .HasColumnName("organisationId");
            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.FallbackMessage)
                .HasColumnName("fallbackMessage");
            builder.Property(o => o.WidgetColor)
                .HasColumnName("widgetColor");
            builder.Property(o => o.WidgetDescription)
                .HasColumnName("widgetDescription");
            builder.Property(o => o.WidgetTitle)
                .HasColumnName("widgetTitle");
            builder.Property(o => o.Enquiries)
                .HasColumnName("enquiries");
            builder.Property(o => o.Domains)
                .HasColumnName("domains");
            builder.Property(o => o.BusinessTimezone)
                .HasColumnName("businessTimezone");
            builder.Property(o => o.BusinessTimeStartUtc)
                .HasColumnName("businessTimeStartUtc");
            builder.Property(o => o.BusinessTimeEndUtc)
                .HasColumnName("businessTimeEndUtc");
            builder.Property(o => o.BusinessEmail)
                .HasColumnName("businessEmail");

            builder.HasMany(o => o.Intents)
                .WithOne(p => p.Project);
            builder.HasMany(o => o.EntityTypes)
                .WithOne(e => e.Project);
            builder.HasMany(o => o.EntityNames)
                .WithOne(e => e.Project);

            builder.HasMany(o => o.Responses)
                .WithOne(o => o.Project)
                .HasForeignKey(o => o.ProjectId)
                .HasPrincipalKey(o => o.Id);

            builder.HasMany(o => o.GreetingResponses)
                .WithOne(o => o.Project);
                
            builder.HasOne(o => o.ProjectVersion);
        }
    }
}
