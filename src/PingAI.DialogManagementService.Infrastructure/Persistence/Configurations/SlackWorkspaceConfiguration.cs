using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class SlackWorkspaceConfiguration : IEntityTypeConfiguration<SlackWorkspace>
    {
        public void Configure(EntityTypeBuilder<SlackWorkspace> builder)
        {
            builder.ToTable("SlackWorkspaces", "chatbot");
            builder.ConfigureId();

            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.OAuthAccessToken)
                .HasColumnName("oauthAccessToken");
            builder.Property(o => o.WebhookUrl)
                .HasColumnName("webhookURL");

            builder.AttachTimestamps();
            
            builder.HasOne(o => o.Project);
        }
    }
}