using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

        private IDbContextTransaction? _currentTransaction;

        public DialogManagementContext(DbContextOptions<DialogManagementContext> options)
            : base(options)
        {
        }
        
        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null) return;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }
        
        public async Task CommitTransactionAsync(Func<Task> beforeCommit)
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction exists.");

            try
            {
                await SaveChangesAsync();
                await beforeCommit();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.EnableDetailedErrors();
            // optionsBuilder.EnableSensitiveDataLogging();
            // optionsBuilder.UseLoggerFactory(GetLoggerFactory(LogLevel.Debug));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                b.Property<Guid>("P1")
                    .HasColumnName("id")
                    .HasDefaultValue(Guid.NewGuid());
                b.AddTimestamps();
            });
            
            modelBuilder.SharedTypeEntity<QueryResponse>(nameof(QueryResponse), b =>
            {
                b.ToTable("QueryResponses", "chatbot");
                b.Property<Guid>("P1")
                    .HasColumnName("id")
                    .HasDefaultValue(Guid.NewGuid());
                b.AddTimestamps();
            });

            modelBuilder.SharedTypeEntity<OrganisationUser>(nameof(OrganisationUser), b =>
            {
                b.ToTable("OrganisationUsers", "chatbot");
                b.Property<Guid>("P1")
                    .HasColumnName("id")
                    .HasDefaultValue(Guid.NewGuid());
                b.AddTimestamps();
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganisationConfiguration).Assembly);
            modelBuilder.ApplyUtcDateTimeConverter();

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
        public DbSet<Intent> Intents { get; set; }
        public DbSet<PhrasePart> PhraseParts { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<EntityValue> EntityValues { get; set; }
        public DbSet<EntityName> EntityNames { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Query> Queries { get; set; }
        public DbSet<SlackWorkspace> SlackWorkspaces { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
                .ToList();

            foreach (var entity in entities)
            {
                var timestamp = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    switch (entity.Entity)
                    {
                        case QueryIntent _:
                        case QueryResponse _:
                        case OrganisationUser _:
                            entity.Property("P1").CurrentValue = Guid.NewGuid();
                            break;
                    }
                    entity.Property("CreatedAt").CurrentValue = timestamp;
                }

                entity.Property("UpdatedAt").CurrentValue = timestamp;
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
