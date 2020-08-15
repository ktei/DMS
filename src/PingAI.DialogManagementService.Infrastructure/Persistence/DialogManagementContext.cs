using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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

        private readonly IMediator _mediator;
        
        public DialogManagementContext(DbContextOptions<DialogManagementContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("chatbot");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganisationConfiguration).Assembly);
            
            NpgsqlConnection.GlobalTypeMapper.MapEnum<IntentType>(pgName: "enum_Intents_type",
                new NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PhrasePartType>(pgName: "enum_PhraseParts_type",
                new NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ResponseType>(pgName: "enum_Response_type",
                new NpgsqlNullNameTranslator());

            // modelBuilder.HasPostgresEnum<IntentType>();
            // modelBuilder.HasPostgresEnum<PhrasePartType>();
            // modelBuilder.HasPostgresEnum<ResponseType>();
            
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
        public DbSet<Query> Queries { get; set; }
        public DbSet<QueryIntent> QueryIntents { get; set; }
        public DbSet<QueryResponse> QueryResponses { get; set; }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            await _mediator.DispatchDomainEvents(this);
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            await _mediator.DispatchDomainEvents(this);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => (x.Entity is IHaveTimestamps || x.Entity is EntityName )&& 
                            (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var timestamp = DateTime.UtcNow;

                if (entity.Entity is IHaveTimestamps)
                {
                    if (entity.State == EntityState.Added)
                    {
                        entity.Property("CreatedAt").CurrentValue = timestamp;
                    }

                    entity.Property("UpdatedAt").CurrentValue = timestamp;
                }
            }
        }
    }
}