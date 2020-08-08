using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class QueryIntentConfiguration :  IEntityTypeConfiguration<QueryIntent>
    {
        public void Configure(EntityTypeBuilder<QueryIntent> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.QueryId)
                .HasColumnName("queryId");
            builder.Property(o => o.IntentId)
                .HasColumnName("intentId");
            
            builder.Property(o => o.CreatedAt)
                .HasColumnName("createdAt");
            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updatedAt");
            
            builder.HasOne(o => o.Query)
                .WithMany(o => o.QueryIntents); 
            builder.HasOne(o => o.Intent)
                .WithMany(o => o.QueryIntents);
        }
    }
}