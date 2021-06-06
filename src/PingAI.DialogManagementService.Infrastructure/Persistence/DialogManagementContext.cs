using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.NameTranslation;
using PingAI.DialogManagementService.Domain.Events;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence.Configurations;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Persistence
{
    public class DialogManagementContext : DbContext
    {
        public const string DefaultSchema = "chatbot";
        static DialogManagementContext() => MapEnums();

        private readonly IMediator _mediator;

        public DialogManagementContext(DbContextOptions<DialogManagementContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.UseLoggerFactory(GetLoggerFactory(LogLevel.Debug));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            // modelBuilder.HasDefaultSchema("chatbot");

            modelBuilder.HasPostgresEnum<IntentType>(DefaultSchema, "enum_Intents_type",
                new NpgsqlNullNameTranslator());
            modelBuilder.HasPostgresEnum<PhrasePartType>(DefaultSchema, "enum_PhraseParts_type",
                new NpgsqlNullNameTranslator());
            modelBuilder.HasPostgresEnum<ResponseType>(DefaultSchema, "enum_Response_type",
                new NpgsqlNullNameTranslator());
            modelBuilder.HasPostgresEnum<SessionStatus>(DefaultSchema, "enum_ChatHistories_session_status",
                new NpgsqlNullNameTranslator());

            modelBuilder.Ignore<DomainEvent>();
            
            modelBuilder.SharedTypeEntity<QueryIntent>(nameof(QueryIntent), b =>
            {
                b.ToTable("QueryIntents", "chatbot");
                b.ConfigureId();
                b.AttachTimestamps();
            });
            
            modelBuilder.SharedTypeEntity<QueryResponse>(nameof(QueryResponse), b =>
            {
                b.ToTable("QueryResponses", "chatbot");
                b.ConfigureId();
                b.AttachTimestamps();
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganisationConfiguration).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        private static void MapEnums()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<IntentType>(pgName: $"{DefaultSchema}.enum_Intents_type",
                new NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PhrasePartType>(pgName: $"{DefaultSchema}.enum_PhraseParts_type",
                new NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ResponseType>(pgName: $"{DefaultSchema}.enum_Response_type",
                new NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<SessionStatus>(
                $"{DefaultSchema}.enum_ChatHistories_session_status", new NpgsqlNullNameTranslator());
        }

        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectVersion> ProjectVersions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrganisationUser> OrganisationUsers { get; set; }
        public DbSet<Intent> Intents { get; set; }
        public DbSet<PhrasePart> PhraseParts { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<EntityValue> EntityValues { get; set; }
        public DbSet<EntityName> EntityNames { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Query> Queries { get; set; }
        // public DbSet<QueryIntent> QueryIntents { get; set; }
        // public DbSet<QueryResponse> QueryResponses { get; set; }
        public DbSet<SlackWorkspace> SlackWorkspaces { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }

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
                .Where(x => (x.Entity is IHaveTimestamps || x.Entity is EntityName) &&
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
        
        private static readonly Func<LogLevel, ILoggerFactory> GetLoggerFactory = logLevel =>
            LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(logLevel)
                    .AddConsole();
            });
    }
}