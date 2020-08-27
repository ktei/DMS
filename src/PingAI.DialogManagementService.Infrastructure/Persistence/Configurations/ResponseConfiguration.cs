using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class ResponseConfiguration : IEntityTypeConfiguration<Response>
    {
        public void Configure(EntityTypeBuilder<Response> builder)
        {
            builder.ToTable("Responses", "chatbot");
            builder.ConfigureId();
            builder.Property(o => o.Resolution)
                .HasColumnName("resolution")
                .HasColumnType("jsonb")
                .HasConversion(x => JsonUtils.Serialize(x, default),
                    x => JsonUtils.Deserialize<ResolutionPart[]>(x, default));
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.Type)
                .HasColumnName("type");
            builder.Property(o => o.Order)
                .HasColumnName("order");
            
            builder.AttachTimestamps();
        }
    }
}