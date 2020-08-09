using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PingAI.DialogManagementService.Infrastructure.Utils
{
    internal static class EntityTypeBuilderExtensions
    {
        public static void AttachTimestamps<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
        {
            builder.Property<DateTime>("CreatedAt").HasColumnName("createdAt");
            builder.Property<DateTime>("UpdatedAt").HasColumnName("updatedAt");
        }
    }
}