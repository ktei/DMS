using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class ProjectVersionConfiguration : IEntityTypeConfiguration<ProjectVersion>
    {
        public void Configure(EntityTypeBuilder<ProjectVersion> builder)
        {
            builder.ToTable("ProjectVersions", "chatbot");
            builder.ConfigureId();
            
            builder.Property(o => o.OrganisationId)
                .HasColumnName("organisationId");
            builder.Property(o => o.Version)
                .HasColumnName("version")
                .HasConversion(v => v.Version, v => new Ver(v));
            builder.Property(o => o.ProjectId)
                .HasColumnName("projectId");
            builder.Property(o => o.VersionGroupId)
                .HasColumnName("versionGroupId");
            
            builder.AttachTimestamps();

            builder.HasOne(o => o.Project);
        }
    }
}