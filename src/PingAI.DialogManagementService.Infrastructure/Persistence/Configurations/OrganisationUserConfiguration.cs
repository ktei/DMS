using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class OrganisationUserConfiguration : IEntityTypeConfiguration<OrganisationUser>
    {
        public void Configure(EntityTypeBuilder<OrganisationUser> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasColumnName("id");
            builder.Property(o => o.OrganisationId)
                .HasColumnName("organisationId");
            builder.Property(o => o.UserId)
                .HasColumnName("userId");
            
            builder.Property(o => o.CreatedAt)
                .HasColumnName("createdAt");
            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updatedAt");


            builder.HasOne(o => o.Organisation)
                .WithMany(o => o.OrganisationUsers);

            builder.HasOne(o => o.User)
                .WithMany(o => o.OrganisationUsers);
        }
    }
}