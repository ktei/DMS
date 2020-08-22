using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class QueryConfiguration:  IEntityTypeConfiguration<Query>
    {
        public void Configure(EntityTypeBuilder<Query> builder)
        {
            builder.ToTable("Queries", "chatbot");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.Name)
                .HasColumnName("name");
            builder.Property(o => o.Expressions)
                .HasColumnName("expressions")
                .HasColumnType("jsonb");
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.Description)
                .HasColumnName("description");
            builder.Property(o => o.Tags)
                .HasColumnName("tags");
            builder.Property(o => o.DisplayOrder)
                .HasColumnName("displayOrder");

            builder.AttachTimestamps();
            
            builder.Ignore(o => o.Intents);
            builder.Ignore(o => o.Responses);
        }
    }
}