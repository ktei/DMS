using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

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
            
            builder.HasMany(x => x.Users)
                .WithMany(x => x.Organisations)
                .UsingEntity<OrganisationUser>(
                    nameof(OrganisationUser),
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("userId")
                        .HasConstraintName("OrganisationUsers_userId_fkey")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Organisation>()
                        .WithMany()
                        .HasForeignKey("organisationId")
                        .HasConstraintName("OrganisationUsers_organisationId_fkey")
                        .OnDelete(DeleteBehavior.Cascade));
        }
    }
}