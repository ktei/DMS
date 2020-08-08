using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class QueryResponseConfiguration : IEntityTypeConfiguration<QueryResponse>
    {
        public void Configure(EntityTypeBuilder<QueryResponse> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.QueryId)
                .HasColumnName("queryId");
            builder.Property(o => o.ResponseId)
                .HasColumnName("responseId");
            
            builder.Property(o => o.CreatedAt)
                .HasColumnName("createdAt");
            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updatedAt");


            builder.HasOne(o => o.Query)
                .WithMany(o => o.QueryResponses); 
            builder.HasOne(o => o.Response)
                .WithMany(o => o.QueryResponses);
        }
    }
}