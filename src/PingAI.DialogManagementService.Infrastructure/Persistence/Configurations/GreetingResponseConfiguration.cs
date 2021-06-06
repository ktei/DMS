using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class GreetingResponseConfiguration : EntityConfigurationBase<GreetingResponse>
    {
        protected override void Configure(EntityTypeBuilder<GreetingResponse> builder)
        {
            builder.ToTable("GreetingResponses", "chatbot");
            
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.ResponseId)
                .HasColumnName("responseId");

            builder.Property(o => o.CreatedAt)
                .HasColumnName("createdAt");
            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updatedAt");
            
            builder.HasOne(o => o.Project)
                .WithMany(o => o.GreetingResponses);
        }
    }
}