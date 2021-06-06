using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class QueryConfiguration:  EntityConfigurationBase<Query>
    {
        protected override void Configure(EntityTypeBuilder<Query> builder)
        {
            builder.ToTable("Queries", "chatbot");
            
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
            
            builder.HasMany(x => x.Intents)
                .WithMany(x => x.Queries)
                .UsingEntity<QueryIntent>(
                    nameof(QueryIntent),
                    j => j
                        .HasOne<Intent>()
                        .WithMany()
                        .HasForeignKey("intentId")
                        .HasConstraintName("QueryIntents_intentId_fkey")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Query>()
                        .WithMany()
                        .HasForeignKey("queryId")
                        .HasConstraintName("QueryIntents_queryId_fkey")
                        .OnDelete(DeleteBehavior.Cascade));
            
            builder.HasMany(x => x.Responses)
                .WithMany(x => x.Queries)
                .UsingEntity<QueryResponse>(
                    nameof(QueryResponse),
                    j => j
                        .HasOne<Response>()
                        .WithMany()
                        .HasForeignKey("responseId")
                        .HasConstraintName("QueryResponses_responseId_fkey")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Query>()
                        .WithMany()
                        .HasForeignKey("queryId")
                        .HasConstraintName("QueryResponses_queryId_fkey")
                        .OnDelete(DeleteBehavior.Cascade));
        }
    }
}
