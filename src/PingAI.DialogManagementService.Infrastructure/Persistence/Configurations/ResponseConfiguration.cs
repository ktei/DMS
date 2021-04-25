using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Utils;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class ResponseConfiguration : IEntityTypeConfiguration<Response>
    {
        public void Configure(EntityTypeBuilder<Response> builder)
        {
            builder.ToTable("Responses", "chatbot");
            builder.ConfigureId();
            builder.Ignore(o => o.Resolution);
            builder.Property(o => o.ResolutionJson)
                .HasColumnName("resolution")
                .HasColumnType("jsonb");
                // .HasConversion(x => SerializeResolution(x),
                //     x => ConvertToResolution(x));
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.Type)
                .HasColumnName("type");
            builder.Property(o => o.Order)
                .HasColumnName("order");
            builder.Property(o => o.SpeechContexts)
                .HasColumnName("speechContexts")
                .HasColumnType("jsonb");
            
            builder.AttachTimestamps();
        }

        private static string? SerializeResolution(Resolution? resolution)
        {
            return resolution == null ? null : JsonUtils.Serialize(resolution);
        }

        private static Resolution? ConvertToResolution(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            // for backward compatibility some of the responses are
            // already using ResolutionPart[]
            var resolutionParts = JsonUtils.TryDeserialize<ResolutionPart[]>(json!);
            if (resolutionParts != null)
            {
                return new Resolution(resolutionParts);
            }

            return JsonUtils.Deserialize<Resolution>(json!);
        }
    }
}