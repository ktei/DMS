using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.NameTranslation;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence.Configurations;

namespace PingAI.DialogManagementService.Infrastructure.Persistence
{
    public class DialogManagementContext : DbContext
    {
        static DialogManagementContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<IntentType>(pgName: "enum_Intents_type",
                new NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PhrasePartType>(pgName: "enum_PhraseParts_type",
                new NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ResponseType>(pgName: "enum_Response_type",
                new NpgsqlNullNameTranslator());
        }
        
        public DialogManagementContext(DbContextOptions<DialogManagementContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("chatbot");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganisationConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrganisationUser> OrganisationUsers { get; set; }
        public DbSet<Intent> Intents { get; set; }
        public DbSet<PhrasePart> PhraseParts { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<EntityValue> EntityValues { get; set; }
        public DbSet<EntityName> EntityNames { get; set; }
        public DbSet<Response> Responses { get; set; }
        
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }
       
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is IHaveTimestamps && 
                            (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    ((IHaveTimestamps)entity.Entity).CreatedAt = now;
                }
                ((IHaveTimestamps)entity.Entity).UpdatedAt = now;
            }
        }
    }
}