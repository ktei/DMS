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
        public string? WidgetColor { get; private set; }
        public string? WidgetDescription { get; private set; }
        public string? FallbackMessage { get; private set; }
        public string[]? Enquiries { get; private set; }
        public ApiKey? ApiKey { get; private set; }
        public string[]? Domains { get; private set; }
        public string BusinessTimezone { get; private set; }
        public DateTime? BusinessTimeStartUtc { get; private set; }
        public DateTime? BusinessTimeEndUtc { get; private set; }
        public string? BusinessEmail { get; private set; }
        
        private DateTime _createdAt;
        public DateTime CreatedAt
        {
            get => DateTime.SpecifyKind(_createdAt, DateTimeKind.Utc);
            private set => _createdAt = value;
        }

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

        private readonly List<GreetingResponse> _greetingResponses;
        public IReadOnlyList<GreetingResponse> GreetingResponses => _greetingResponses.ToImmutableList();
        
        public ProjectVersion ProjectVersion { get; private set; }

        public const int MaxNameLength = 250;

        public Project(Cache cache) : this(cache.Name, cache.OrganisationId,
            cache.WidgetTitle, cache.WidgetColor!, cache.WidgetDescription, cache.FallbackMessage, 
            cache.Enquiries,
            cache.Domains, cache.BusinessTimezone, cache.BusinessTimeStartUtc,
            cache.BusinessTimeEndUtc, cache.BusinessEmail)
        {
            Id = cache.Id;

            var greetingResponses = new List<Response>();
            if (cache.GreetingMessage != null)
            {
                var r = new Response(ResponseType.RTE, 0);
                r.SetRteText(cache.GreetingMessage, new Dictionary<string, EntityName>());
                greetingResponses.Add(r);
            }

            var order = 1;
            foreach (var qr in cache.QuickReplies ?? new string[0])
            {
                var r = new Response(ResponseType.QUICK_REPLY, order++);
                r.SetRteText(qr, new Dictionary<string, EntityName>());
                greetingResponses.Add(r);
            }
            UpdateGreetingResponses(greetingResponses);
        }

        public Project(string name, Guid organisationId, string? widgetTitle, string widgetColor,
            string? widgetDescription, string? fallbackMessage, string[]? enquiries,
            string[]? domains, string businessTimezone, DateTime? businessTimeStartUtc,
            DateTime? businessTimeEndUtc, string? businessEmail)
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
            Enquiries = enquiries;
            Domains = domains;
            BusinessTimezone = businessTimezone;
            BusinessTimeStartUtc = businessTimeStartUtc;
            BusinessTimeEndUtc = businessTimeEndUtc;
            BusinessEmail = businessEmail;
            _entityTypes = new List<EntityType>();
            _entityNames = new List<EntityName>();
            _intents = new List<Intent>();
            _responses = new List<Response>();
            _queries = new List<Query>();
            _greetingResponses = new List<GreetingResponse>();

            if (ApiKey == null || ApiKey.IsEmpty)
            {
                GenerateNewApiKey();
            }
        }

        public void GenerateNewApiKey()
        {
            ApiKey = ApiKey.GenerateNew();
        }

        public void UpdateWidgetTitle(string widgetTitle)
        {
            if (!string.IsNullOrEmpty(widgetTitle) && widgetTitle.Length > 255)
                throw new BadRequestException($"{nameof(widgetTitle)}'s length cannot exceed 255");
            if (WidgetTitle == widgetTitle) return;
            WidgetTitle = widgetTitle;
            AddProjectUpdatedEvent();
        }

        public void UpdateWidgetColor(string widgetColor)
        {
            if (string.IsNullOrWhiteSpace(widgetColor))
                throw new BadRequestException($"{nameof(widgetColor)} cannot be empty");
            if (WidgetColor == widgetColor) return;
            WidgetColor = widgetColor;
            AddProjectUpdatedEvent();
        }

        public void UpdateWidgetDescription(string widgetDescription)
        {
            if (WidgetDescription == widgetDescription)
                return;
            WidgetDescription = widgetDescription;
            AddProjectUpdatedEvent();
        }

        public void UpdateFallbackMessage(string fallbackMessage)
        {
            if (FallbackMessage == fallbackMessage)
                return;
            FallbackMessage = fallbackMessage;
            AddProjectUpdatedEvent();
        }

        public void UpdateGreetingResponses(IEnumerable<Response> responses)
        {
            _greetingResponses.Clear();
            foreach (var r in responses)
            {
                AddGreetingResponse(r);
            }
            AddProjectUpdatedEvent();
        }

        public void UpdateDomains(string[]? domains)
        {
            if (Domains?.SequenceEqual(domains ?? new string[]{}) == true)
                return;
            Domains = (domains ?? new string[] { })
                .Select(x => (x ?? "").Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToArray();
            AddProjectUpdatedEvent();
        }

        public void UpdateEnquiries(string[]? enquiries)
        {
            if (Domains?.SequenceEqual(enquiries ?? new string[]{}) == true)
                return;

            Enquiries = (enquiries ?? new string[]{})
                .OrderBy(e => e)
                .Distinct().ToArray();
            AddProjectUpdatedEvent();
        }

        public void UpdateBusinessHours(DateTime businessTimeStartUtc, DateTime businessTimeEndUtc,
            string businessTimezone = Defaults.BusinessTimezone)
        {
            if (string.IsNullOrWhiteSpace(businessTimezone))
                throw new ArgumentException($"{nameof(businessTimezone)} cannot be empty");
            if (businessTimeEndUtc <= businessTimeStartUtc)
                throw new ArgumentException($"{nameof(businessTimeEndUtc)} " +
                                            $"must be greater than {nameof(businessTimeStartUtc)}");
            if (BusinessTimeStartUtc == businessTimeStartUtc &&
                BusinessTimeEndUtc == businessTimeEndUtc &&
                BusinessTimezone == businessTimezone)
                return;
            
            BusinessTimeStartUtc = businessTimeStartUtc;
            BusinessTimeEndUtc = businessTimeEndUtc;
            BusinessTimezone = businessTimezone;
            
            AddProjectUpdatedEvent();
        }

        public void UpdateBusinessEmail(string businessEmail)
        {
            if (string.IsNullOrWhiteSpace(businessEmail))
                throw new ArgumentException($"{nameof(businessEmail)} cannot be empty");
            if (BusinessEmail == businessEmail)
                return;
            
            BusinessEmail = businessEmail;
            AddProjectUpdatedEvent();
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

        private void AddGreetingResponse(Response response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            if (_greetingResponses == null)
                throw new ArgumentNullException($"Load {nameof(GreetingResponses)} first");
            _greetingResponses.Add(new GreetingResponse(this, response));
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
            if (_greetingResponses == null)
                throw new InvalidOperationException($"Load {nameof(GreetingResponses)} before publishing");
            
            var entityNamesCopy =
                _entityNames.ToDictionary(n => n.Id, 
                    n => new EntityName(n.Name, Guid.Empty, n.CanBeReferenced));
            var entityTypesCopy =
                _entityTypes.ToDictionary(e => e.Id,
                    e => new EntityType(e.Name, Guid.Empty,
                        e.Description,
                        e.Values.Select(v => new EntityValue(v.Value, Guid.Empty, v.Synonyms))));

            PhrasePart CopyPhrasePart(Intent i, PhrasePart p) =>
                new PhrasePart(i.Id, p.PhraseId, p.Position,
                    p.Text, p.Value, p.Type, p.EntityNameId.HasValue ? entityNamesCopy[p.EntityNameId.Value] : default,
                    p.EntityTypeId.HasValue ? entityTypesCopy[p.EntityTypeId.Value] : default, p.DisplayOrder);

            Intent CopyIntent(Intent i)
            {
                var intent = new Intent(i.Name, Guid.Empty, i.Type, i.PhraseParts.Select(p => CopyPhrasePart(i, p)));
                intent.ClearDomainEvents(); // we don't want to trigger IntentUpdatedEvent;
                                            // we only want to copy intents to a different DMS project
                return intent;
            }

            static Response CopyResponse(Response r) =>
                new Response(r.Resolution, Guid.Empty, r.Type, r.Order);
            
            var projectToPublish = new Project($"{Name}__{Guid.NewGuid()}",
                OrganisationId, 
                WidgetTitle, WidgetColor!,
                WidgetDescription, FallbackMessage, 
                Enquiries, Domains?.ToArray(), BusinessTimezone, BusinessTimeStartUtc,
                BusinessTimeEndUtc, BusinessEmail);

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
                foreach (var qi in query.QueryIntents)
                {
                    q.AddIntent(intentsCopy[qi.IntentId]);
                }

                foreach (var qr in query.QueryResponses)
                {
                    q.AddResponse(responsesCopy[qr.ResponseId]);
                }
                projectToPublish.AddQuery(q);
            }

            foreach (var greetingResponse in _greetingResponses)
            {
                if (greetingResponse.Response == null)
                    throw new InvalidOperationException($"{nameof(greetingResponse.Response)} must be loaded");
                var responseCopy = responsesCopy[greetingResponse.Response.Id]; // CopyResponse(greetingResponse.Response!);
                projectToPublish.AddGreetingResponse(responseCopy);
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

        private void AddProjectUpdatedEvent()
        {
            if (!DomainEvents.Any(e => e is ProjectUpdatedEvent))
            {
                AddDomainEvent(new ProjectUpdatedEvent(this));
            }
        }
        
        public override string ToString() => Name;
        
        public class Cache
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public Guid OrganisationId { get; set; }
            public string? WidgetTitle { get; set; }
            public string? WidgetColor { get; set; }
            public string? WidgetDescription { get; set; }
            public string? FallbackMessage { get; set; }
            public string? GreetingMessage { get; set; }
            public string[]? QuickReplies { get; set; }
            public string[]? Enquiries { get; set; }
            public ApiKey? ApiKey { get; set; }
            public string[]? Domains { get; set; }
            public string BusinessTimezone { get; set; }
            public DateTime? BusinessTimeStartUtc { get; set; }
            public DateTime? BusinessTimeEndUtc { get; set; }
            public string? BusinessEmail { get; set; }
            public DateTime CreatedAt { get; set; }

            public static string MakeKey(Guid projectId) => $"{nameof(Project)}_{projectId}";

            public Cache(Project project)
            {
                Id = project.Id;
                Name = project.Name;
                OrganisationId = project.OrganisationId;
                WidgetTitle = project.WidgetTitle;
                WidgetColor = project.WidgetColor;
                WidgetDescription = project.WidgetDescription;
                FallbackMessage = project.FallbackMessage;
                Enquiries = project.Enquiries;
                ApiKey = project.ApiKey;
                Domains = project.Domains;
                BusinessTimezone = project.BusinessTimezone;
                BusinessTimeStartUtc = project.BusinessTimeStartUtc;
                BusinessTimeEndUtc = project.BusinessTimeEndUtc;
                BusinessEmail = project.BusinessEmail;
                
                GreetingMessage = project.GreetingResponses.FirstOrDefault(gr => 
                        gr.Response?.Type == ResponseType.RTE)?
                    .Response?.GetDisplayText();
                var quickReplies = new List<string>();
                foreach (var gr in project.GreetingResponses
                    .Where(x => x.Response!.Type == ResponseType.QUICK_REPLY)
                    .OrderBy(x => x.Response!.Order))
                {
                    quickReplies.Add(gr.Response!.GetDisplayText());
                }

                QuickReplies = quickReplies.ToArray();

            }

            public Cache()
            {
                
            }
        }
    }
}