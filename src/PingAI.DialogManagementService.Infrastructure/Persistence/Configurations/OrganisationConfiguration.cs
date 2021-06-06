using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class OrganisationConfiguration : EntityConfigurationBase<Organisation>
    {
        protected override void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder.ToTable("Organisations", "chatbot");

            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.Description)
                .HasColumnName("description");
            builder.Property(o => o.Tags)
                .HasColumnName("tags");
            
            builder.HasMany(o => o.Projects)
                .WithOne(p => p.Organisation);
            builder.HasMany(o => o.ProjectVersions)
                .WithOne(p => p.Organisation);
        }
    }
}