using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PingAI.DialogManagementService.Infrastructure.Utils
{
    public static class EntityTypeBuilderExtensions
    {
        public static void AddId<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
        {
            builder.Property<Guid>("Id")
                .HasColumnName("id");
            builder.HasKey("Id");
        }

        public static void AddTimestamps<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
        {
            builder.Property<DateTime>("CreatedAt")
                .HasColumnName("createdAt")
                .HasDefaultValue(DateTime.UtcNow);
            builder.Property<DateTime>("UpdatedAt")
                .HasColumnName("updatedAt")
                .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
