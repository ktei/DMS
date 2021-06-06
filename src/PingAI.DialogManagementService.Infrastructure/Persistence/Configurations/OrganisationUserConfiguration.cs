using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class OrganisationUserConfiguration : EntityConfigurationBase<OrganisationUser>
    {
        protected override void Configure(EntityTypeBuilder<OrganisationUser> builder)
        {
            builder.ToTable("OrganisationUsers", "chatbot");

            builder.Property(o => o.OrganisationId)
                .HasColumnName("organisationId");
            builder.Property(o => o.UserId)
                .HasColumnName("userId");
            
            builder.HasOne(o => o.Organisation)
                .WithMany(o => o.OrganisationUsers);

            builder.HasOne(o => o.User)
                .WithMany(o => o.OrganisationUsers);
        }
    }
}