using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PingAI.DialogManagementService.Infrastructure.Utils
{
    internal static class EntityTypeBuilderExtensions
    {
        public static void ConfigureId<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
        {
            builder.HasKey("Id");
            builder.Property<Guid>("Id").HasColumnName("id");
        }
        
        public static void AttachTimestamps<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
        {
            builder.Property<DateTime>("CreatedAt").HasColumnName("createdAt");
            builder.Property<DateTime>("UpdatedAt").HasColumnName("updatedAt");
        }
    }
}