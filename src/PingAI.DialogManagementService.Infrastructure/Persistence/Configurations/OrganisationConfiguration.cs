using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder.ToTable("Organisations", "chatbot");

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
            
            builder.AttachTimestamps();
            
            builder.HasMany(o => o.Projects)
                .WithOne(p => p.Organisation);
            builder.HasMany(o => o.ProjectVersions)
                .WithOne(p => p.Organisation);
        }
    }
}