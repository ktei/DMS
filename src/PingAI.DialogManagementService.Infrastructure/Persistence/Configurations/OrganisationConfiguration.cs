using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.Description)
                .HasColumnName("description");
            builder.Property(o => o.Tags)
                .HasColumnName("tags");
            
            builder.Property(o => o.CreatedAt)
                .HasColumnName("createdAt");
            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updatedAt");

            builder.HasMany(o => o.Projects)
                .WithOne(p => p.Organisation);
        }
    }
}