using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Events;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Project : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid OrganisationId { get; private set; }
        public Organisation? Organisation { get; private set; }
        public string? WidgetTitle { get; private set; }

        private string? _widgetColor;
        public string? WidgetColor
        {
            get => (_widgetColor ?? string.Empty).TrimEnd();
            private set => _widgetColor = value;
        }
        
        public string? WidgetDescription { get; private set; }
        public string? FallbackMessage { get; private set; }
        public string? GreetingMessage { get; private set; }
        public string[]? Enquiries { get; private set; }

        private readonly List<Intent> _intents;
        public IReadOnlyList<Intent> Intents => _intents.ToImmutableList();

        private readonly List<EntityType> _entityTypes;
        public IReadOnlyList<EntityType> EntityTypes => _entityTypes.ToImmutableList();

        private readonly List<EntityName> _entityNames;
        public IReadOnlyList<EntityName> EntityNames => _entityNames.ToImmutableList();

        private readonly List<Response> _responses;
        public IReadOnlyList<Response> Responses => _responses.ToImmutableList();

        private readonly List<Query> _queries;
        public IReadOnlyList<Query> Queries => _queries.ToImmutableList();

        public const int MaxNameLength = 250;

        public Project(string name, Guid organisationId, string? widgetTitle, string widgetColor,
            string? widgetDescription, string? fallbackMessage, string? greetingMessage, string[]? enquiries)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");
            if (name.Trim().Length > MaxNameLength)
                throw new ArgumentException($"Max length of {nameof(name)} is {MaxNameLength}");
            
            if (string.IsNullOrWhiteSpace(widgetColor))
                throw new ArgumentException($"{nameof(widgetColor)} cannot be empty");
            
            Name = name.Trim();
            OrganisationId = organisationId;
            WidgetTitle = widgetTitle;
            WidgetColor = widgetColor;
            WidgetDescription = widgetDescription;
            FallbackMessage = fallbackMessage;
            GreetingMessage = greetingMessage;
            Enquiries = enquiries;
            _entityTypes = new List<EntityType>();
            _entityNames = new List<EntityName>();
            _intents = new List<Intent>();
            _responses = new List<Response>();
            _queries = new List<Query>();
        }

        public void UpdateWidgetTitle(string widgetTitle)
        {
            if (!string.IsNullOrEmpty(widgetTitle) && widgetTitle.Length > 255)
                throw new BadRequestException($"{nameof(widgetTitle)}'s length cannot exceed 255");
            WidgetTitle = widgetTitle;
        }

        public void UpdateWidgetColor(string widgetColor)
        {
            if (string.IsNullOrWhiteSpace(widgetColor))
                throw new BadRequestException($"{nameof(widgetColor)} cannot be empty");
            WidgetColor = widgetColor; 
        }

        public void UpdateWidgetDescription(string widgetDescription)
        {
            WidgetDescription = widgetDescription;
        }

        public void UpdateFallbackMessage(string fallbackMessage)
        {
            FallbackMessage = fallbackMessage;
        }

        public void UpdateGreetingMessage(string greetingMessage)
        {
            GreetingMessage = greetingMessage;
        }

        public void UpdateEnquiries(string[]? enquiries)
        {
            Enquiries = (enquiries ?? new string[]{})
                .OrderBy(e => e)
                .Distinct().ToArray();
        }

        public void AddIntent(Intent intent)
        {
            _ = intent ?? throw new ArgumentNullException(nameof(intent));
            if (_intents == null)
                throw new InvalidOperationException($"Load {nameof(Intent)} first");
            _intents.Add(intent);
        }

        public void AddQuery(Query query)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));
            if (_queries == null)
                throw new InvalidOperationException($"Load {nameof(Queries)} first");
            _queries.Add(query);
        }

        public void AddEntityType(EntityType entityType)
        {
            _ = entityType ?? throw new ArgumentNullException(nameof(entityType));
            if (_entityTypes == null)
                throw new InvalidOperationException($"Load {nameof(EntityTypes)} first");
            _entityTypes.Add(entityType);
        }

        public void AddEntityName(EntityName entityName)
        {
            _ = entityName ?? throw new ArgumentNullException(nameof(entityName));
            if (_entityNames == null)
                throw new InvalidOperationException($"Load {nameof(EntityNames)} first");
            _entityNames.Add(entityName);
        }

        public void AddResponse(Response response)
        {
            _ = _responses ?? throw new ArgumentNullException(nameof(response));
            if (_responses == null)
                throw new InvalidOperationException($"Load {nameof(Responses)} first");
            _responses.Add(response);
        }

        /// <summary>
        /// Export current project by copying it to a new project and return the copy
        /// </summary>
        /// <returns>The project copy</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Project Export()
        {
            if (Id == Guid.Empty)
            {
                throw new InvalidOperationException("Transient project cannot be published");
            }

            if (_entityNames == null)
                throw new InvalidOperationException($"Load {nameof(EntityNames)} before publishing");
            if (_entityTypes == null)
                throw new InvalidOperationException($"Load {nameof(EntityTypes)} before publishing");
            if (_intents == null)
                throw new InvalidOperationException($"Load {nameof(Intents)} before publishing");
            if (_responses == null)
                throw new InvalidOperationException($"Load {nameof(Responses)} before publishing");
            if (_queries == null)
                throw new InvalidOperationException($"Load {nameof(Queries)} before publishing");
            
            var entityNamesCopy =
                _entityNames.ToDictionary(n => n.Id, 
                    n => new EntityName(n.Name, Guid.Empty, n.CanBeReferenced));
            var entityTypesCopy =
                _entityTypes.ToDictionary(e => e.Id,
                    e => new EntityType(e.Name, Guid.Empty,
                        e.Description, e.Tags?.ToArray(), 
                        e.Values.Select(v => new EntityValue(v.Value, Guid.Empty, v.Synonyms))));

            PhrasePart CopyPhrasePart(Intent i, PhrasePart p) =>
                new PhrasePart(i.Id, p.PhraseId, p.Position,
                    p.Text, p.Value, p.Type, p.EntityNameId.HasValue ? entityNamesCopy[p.EntityNameId.Value] : default,
                    p.EntityTypeId.HasValue ? entityTypesCopy[p.EntityTypeId.Value] : default);

            Intent CopyIntent(Intent i) =>
                new Intent(i.Name, Guid.Empty, i.Type, i.PhraseParts.Select(p => CopyPhrasePart(i, p)));

            static Response CopyResponse(Response r) =>
                new Response(r.Resolution.ToArray(), Guid.Empty, r.Type, r.Order);
            
            var projectToPublish = new Project($"{Name}__{Guid.NewGuid()}",
                OrganisationId, 
                WidgetTitle, WidgetColor!,
                WidgetDescription, FallbackMessage, GreetingMessage, Enquiries);

            var intentsCopy = _intents.ToDictionary(i => i.Id, CopyIntent);
            var responsesCopy = _responses.ToDictionary(r => r.Id, CopyResponse);

            foreach (var entityName in _entityNames)
            {
                projectToPublish.AddEntityName(entityNamesCopy[entityName.Id]);
            }

            foreach (var entityType in _entityTypes)
            {
                projectToPublish.AddEntityType(entityTypesCopy[entityType.Id]);
            }
            
            foreach (var intent in _intents)
            {
                projectToPublish.AddIntent(intentsCopy[intent.Id]);
            }

            foreach (var response in _responses)
            {
                projectToPublish.AddResponse(responsesCopy[response.Id]);
            }

            foreach (var query in _queries)
            {
                var q = new Query(query.Name, Guid.Empty,
                    query.Expressions.ToArray(), query.Description,
                    query.Tags?.ToArray(), query.DisplayOrder);
                foreach (var i in query.Intents)
                {
                    q.AddIntent(intentsCopy[i.Id]);
                }

                foreach (var r in query.Responses)
                {
                    q.AddResponse(responsesCopy[r.Id]);
                }
                projectToPublish.AddQuery(q);
            }
            
            AddProjectPublishedEvent(projectToPublish);
            
            return projectToPublish;
        }

        private void AddProjectPublishedEvent(Project publishedProjectId)
        {
            if (!DomainEvents.Any(e => e is ProjectPublishedEvent))
            {
                AddDomainEvent(new ProjectPublishedEvent(this, publishedProjectId));
            }
        }
        
        public override string ToString() => Name;
    }
}