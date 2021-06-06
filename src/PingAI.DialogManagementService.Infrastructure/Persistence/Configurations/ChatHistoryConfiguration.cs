using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class ChatHistoryConfiguration : EntityConfigurationBase<ChatHistory>
    {
        protected override void Configure(EntityTypeBuilder<ChatHistory> builder)
        {
            builder.ToTable("ChatHistories", "chatbot");

            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.SessionId)
                .HasColumnName("sessionId");
            builder.Property(o => o.Input)
                .HasColumnName("input")
                .HasColumnType("jsonb");
            builder.Property(o => o.Output)
                .HasColumnName("output")
                .HasColumnType("jsonb");
            builder.Property(o => o.RequestId)
                .HasColumnName("requestId");
            builder.Property(o => o.SessionStatus)
                .HasColumnName("sessionStatus");
            builder.Ignore(o => o.ChatHistoryInput);
            builder.Ignore(o => o.ChatHistoryOutput);
        }
    }
}
