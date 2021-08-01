using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Events;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Project : DomainEntity
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
        public string[]? Domains { get; private set; }
        public string BusinessTimezone { get; private set; }
        public DateTime? BusinessTimeStartUtc { get; private set; }
        public DateTime? BusinessTimeEndUtc { get; private set; }
        public string? BusinessEmail { get; private set; }
        public DateTime CreatedAt { get; private set; }

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
        public ProjectVersion? ProjectVersion { get; private set; }

        public const int MaxNameLength = 250;

        public Project(Guid id, Guid organisationId, string name, string? widgetTitle, string widgetColor,
            string? widgetDescription, string? fallbackMessage, string[]? enquiries,
            string[]? domains, string businessTimezone, DateTime? businessTimeStartUtc,
            DateTime? businessTimeEndUtc, string? businessEmail) : this(organisationId, name, widgetTitle,
            widgetColor, widgetDescription, fallbackMessage, enquiries, domains, businessTimezone,
            businessTimeStartUtc, businessTimeEndUtc, businessEmail)
        {
            Id = id;
        }

        public Project(Guid organisationId, string name, string? widgetTitle, string widgetColor,
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

            Id = Guid.NewGuid();
            OrganisationId = organisationId;
            Name = name.Trim();
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
        }

        public static Project CreateWithDefaults(Guid organisationId, string name) =>
            new Project(organisationId, name,
                Defaults.WidgetTitle, Defaults.WidgetColor,
                Defaults.WidgetDescription, Defaults.FallbackMessage,
                null, null, Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                Defaults.BusinessTimeEndUtc, null);
        
        public static Project FromCache(ProjectCache projectCache)
        {
            var project = new Project(
                projectCache.OrganisationId, 
                projectCache.Name,
                projectCache.WidgetTitle, 
                projectCache.WidgetColor!, 
                projectCache.WidgetDescription,
                projectCache.FallbackMessage,
                projectCache.Enquiries,
                projectCache.Domains, 
                projectCache.BusinessTimezone, 
                projectCache.BusinessTimeStartUtc,
                projectCache.BusinessTimeEndUtc, 
                projectCache.BusinessEmail);
            
            var greetingResponses = new List<Response>();
            if (projectCache.GreetingMessage != null)
            {
                var r = new Response(Resolution.Factory.RteText(projectCache.GreetingMessage), ResponseType.RTE, 0);
                greetingResponses.Add(r);
            }

            var order = 1;
            foreach (var qr in projectCache.QuickReplies ?? new string[0])
            {
                var r = new Response(Resolution.Factory.RteText(qr), ResponseType.QUICK_REPLY, order++);
                greetingResponses.Add(r);
            }

            project._greetingResponses.AddRange(
                greetingResponses.Select(gr => new GreetingResponse(project, gr)));

            return project;
        }

        public void MarkAsDesignTime()
        {
            ProjectVersion = ProjectVersion.NewDesignTime(this);
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

        public void AddGreetingResponse(Response response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            if (_greetingResponses == null)
                throw new ArgumentNullException($"Load {nameof(GreetingResponses)} first");
            if (!_responses.Contains(response))
            {
                _responses.Add(response);
            }
            _greetingResponses.Add(new GreetingResponse(this, response));
        }

        /// <summary>
        /// Import the given project by copying all its child entities such as intents,
        /// queries, responses etc.
        /// </summary>
        public void Import(Project target)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            if (Id == Guid.Empty)
            {
                throw new InvalidOperationException("Project must have a non-empty (GUID) Id " +
                                                    "before importing another project.");
            }

            if (target.Id == Guid.Empty)
                throw new InvalidOperationException($"Target project must have a non-empty (GUID) Id.");

            if (target.EntityNames == null)
                throw new InvalidOperationException($"Load {nameof(target)}.{nameof(EntityNames)} before importing.");
            if (target.EntityTypes == null)
                throw new InvalidOperationException($"Load {nameof(target)}.{nameof(EntityTypes)} before importing.");
            if (target.Intents == null)
                throw new InvalidOperationException($"Load {nameof(target)}.{nameof(Intents)} before importing.");
            if (target.Responses == null)
                throw new InvalidOperationException($"Load {nameof(target)}.{nameof(Responses)} before importing.");
            if (target.Queries == null)
                throw new InvalidOperationException($"Load {nameof(target)}.{nameof(Queries)} before importing.");
            if (target.GreetingResponses == null)
                throw new InvalidOperationException($"Load {nameof(target)}.{nameof(GreetingResponses)} before importing.");
            
            CopyProperties(target);
            var entityNames = CopyEntityNames(target);
            var entityTypes = CopyEntityTypes(target);
            var intents = CopyIntents(target, entityNames, entityTypes);
            var responses = CopyResponses(target);
            CopyQueries(target, intents, responses);
            CopyGreetingResponses(target, responses);
        }

        private void CopyProperties(Project target)
        {
            WidgetTitle = target.WidgetTitle;
            WidgetColor = target.WidgetColor;
            WidgetDescription = target.WidgetDescription;
            FallbackMessage = target.FallbackMessage;
            Enquiries = target.Enquiries;
            Domains = target.Domains;
            BusinessTimezone = target.BusinessTimezone;
            BusinessTimeStartUtc = target.BusinessTimeStartUtc;
            BusinessTimeEndUtc = target.BusinessTimeEndUtc;
            BusinessEmail = target.BusinessEmail;
        }

        private Dictionary<Guid, EntityName> CopyEntityNames(Project target)
        {
            var entityNameMap = new Dictionary<Guid, EntityName>();
            foreach (var entityName in target.EntityNames)
            {
                var copy = new EntityName(Id, entityName.Name, entityName.CanBeReferenced);
                AddEntityName(copy);
                entityNameMap.Add(entityName.Id, copy);
            }

            return entityNameMap;
        }
        
        private Dictionary<Guid, EntityType> CopyEntityTypes(Project target)
        {
            var entityTypeMap = new Dictionary<Guid, EntityType>();
            foreach (var entityType in target.EntityTypes)
            {
                var copy = new EntityType(Id, entityType.Name, entityType.Description);
                AddEntityType(copy);
                entityTypeMap.Add(entityType.Id, copy);
            }

            return entityTypeMap;
        }

        private Dictionary<Guid, Intent> CopyIntents(Project target,
            Dictionary<Guid, EntityName> entityNames,
            Dictionary<Guid, EntityType> entityTypes)
        {
            var intentMap = new Dictionary<Guid, Intent>();
            foreach (var intent in target.Intents)
            {
                var copy = new Intent(Id, intent.Name, intent.Type);
                foreach (var phrasePart in intent.PhraseParts)
                {
                    var partCopy = new PhrasePart(phrasePart.PhraseId,
                        phrasePart.Position, phrasePart.Text,
                        phrasePart.Value, phrasePart.Type,
                        phrasePart.EntityNameId.HasValue ? entityNames[phrasePart.EntityNameId.Value] : null,
                        phrasePart.EntityTypeId.HasValue ? entityTypes[phrasePart.EntityTypeId.Value] : null,
                        phrasePart.DisplayOrder);
                    copy.AddPhrasePart(partCopy);
                }
                intentMap.Add(intent.Id, copy);
                AddIntent(copy);
            }

            return intentMap;
        }

        private Dictionary<Guid, Response> CopyResponses(Project target)
        {
            var responseMap = new Dictionary<Guid, Response>();
            foreach (var response in target.Responses)
            {
                var copy = new Response(Id, response.Resolution, response.Type, response.Order);
                responseMap.Add(response.Id, copy);
                AddResponse(copy);
            }

            return responseMap;
        }

        private void CopyQueries(Project target, Dictionary<Guid, Intent> intents,
            Dictionary<Guid, Response> responses)
        {
            foreach (var query in target.Queries)
            {
                var copy = new Query(Id, query.Name, query.Expressions,
                    query.Description, query.Tags, query.DisplayOrder);
                foreach (var intent in query.Intents)
                {
                    copy.AddIntent(intents[intent.Id]);
                }

                foreach (var response in query.Responses)
                {
                    copy.AddResponse(responses[response.Id]);
                }

                AddQuery(copy);
            }
        }

        private void CopyGreetingResponses(Project target, Dictionary<Guid, Response> responses)
        {
            foreach (var greetingResponse in target.GreetingResponses)
            {
                var copy = new GreetingResponse(this, responses[greetingResponse.ResponseId]);
                _greetingResponses.Add(copy);
            }
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
    }
}
