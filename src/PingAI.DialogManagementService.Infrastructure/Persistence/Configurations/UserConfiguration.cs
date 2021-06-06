using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : EntityConfigurationBase<User>
    {
        protected override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "chatbot");
            
            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(o => o.Auth0Id)
                .HasColumnName("auth0Id")
                .HasMaxLength(255);
        }
    }
}