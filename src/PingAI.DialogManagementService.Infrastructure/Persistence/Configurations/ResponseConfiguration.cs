using System;
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
                .HasConversion(x => SerializeResolution(x),
                    x => ConvertToResolution(x));
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.Type)
                .HasColumnName("type");
            builder.Property(o => o.Order)
                .HasColumnName("order");
            
            builder.AttachTimestamps();
        }

        private static string? SerializeResolution(Resolution? resolution)
        {
            if (resolution == null)
                return null;
            return resolution.Type switch
            {
                ResolutionType.PARTS => JsonUtils.Serialize(resolution.Parts!),
                _ => JsonUtils.Serialize(resolution)
            };
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