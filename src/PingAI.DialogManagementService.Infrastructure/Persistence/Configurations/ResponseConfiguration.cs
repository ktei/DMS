using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class ResponseConfiguration : IEntityTypeConfiguration<Response>
    {
        public void Configure(EntityTypeBuilder<Response> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.Resolution)
                .HasColumnName("resolution")
                .HasColumnType("jsonb");
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.Type)
                .HasColumnName("type");
            builder.Property(o => o.Order)
                .HasColumnName("order");
            
            builder.Property(o => o.CreatedAt)
                .HasColumnName("createdAt");
            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updatedAt");
        }
    }
}