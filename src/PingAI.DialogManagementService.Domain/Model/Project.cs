using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using PingAI.DialogManagementService.Domain.ErrorHandling;

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
        private static readonly Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$");

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
            ProjectVersion = ProjectVersion.NewDesignTime(this);
            _entityTypes = new List<EntityType>();
            _entityNames = new List<EntityName>();
            _intents = new List<Intent>();
            _responses = new List<Response>();
            _queries = new List<Query>();
            _greetingResponses = new List<GreetingResponse>();
        }

        public static Project CreateWithDefaults(Guid organisationId, string name, bool withGreetingMessage = true)
        {
            var project = new Project(organisationId, name,
                Defaults.WidgetTitle, Defaults.WidgetColor,
                Defaults.WidgetDescription, Defaults.FallbackMessage,
                null, null, Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                Defaults.BusinessTimeEndUtc, null);
            if (withGreetingMessage)
                project.SetGreetingMessage(Defaults.GreetingMessage);
            return project;
        }

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

        public void CustomiseWidget(string widgetTitle,
            string widgetColor, string widgetDescription)
        {
            if (!string.IsNullOrEmpty(widgetTitle) && widgetTitle.Length > 255)
                throw new BadRequestException($"{nameof(widgetTitle)}'s length cannot exceed 255");
            if (string.IsNullOrWhiteSpace(widgetColor))
                throw new BadRequestException($"{nameof(widgetColor)} cannot be empty");
            WidgetTitle = widgetTitle;
            WidgetColor = widgetColor;
            WidgetDescription = widgetDescription;
        }
        
        public void SetFallbackMessage(string fallbackMessage)
        {
            if (string.IsNullOrWhiteSpace(fallbackMessage))
                throw new BadRequestException($"{nameof(fallbackMessage)} cannot be empty");
            FallbackMessage = fallbackMessage;
        }

        public Response[] SetGreetingMessage(string? greetingMessage)
        {
            var existingGreetings = _greetingResponses
                .Where(gr => gr.Response!.Type == ResponseType.RTE)
                .ToList();
            foreach (var existingGreeting in existingGreetings)
            {
                _greetingResponses.Remove(existingGreeting);
            }

            if (greetingMessage != null)
            {
                var response = new Response(Id, Resolution.Factory.RteText(greetingMessage), ResponseType.RTE,
                    0);
                AddGreetingResponse(response);
            }

            return existingGreetings.Select(gr => gr.Response!).ToArray();
        }

        public Response[] SetQuickReplies(IEnumerable<string> quickReplies)
        {
            if (quickReplies == null)
                throw new ArgumentNullException(nameof(quickReplies));
            if (_greetingResponses == null)
                throw new InvalidOperationException($"Load {Responses} first.");
            var existingQuickReplies = _greetingResponses
                .Where(gr => gr.Response!.Type == ResponseType.QUICK_REPLY)
                .ToList();
            foreach (var existingQuickReply in existingQuickReplies)
            {
                _greetingResponses.Remove(existingQuickReply);
            }
            
            var responseOrder = 1;
            foreach (var quickReply in quickReplies)
            {
                var response = new Response(Id, Resolution.Factory.RteText(quickReply), ResponseType.QUICK_REPLY,
                    responseOrder++);
                AddGreetingResponse(response);
            }

            return existingQuickReplies.Select(gr => gr.Response!).ToArray();
        }

        public void SetDomains(string[]? domains)
        {
            if (Domains?.SequenceEqual(domains ?? new string[]{}) == true)
                return;
            Domains = (domains ?? new string[] { })
                .Select(x => (x ?? "").Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToArray();
        }

        public void SetBusinessHours(DateTime businessTimeStartUtc, DateTime businessTimeEndUtc,
            string businessTimezone = Defaults.BusinessTimezone)
        {
            if (string.IsNullOrWhiteSpace(businessTimezone))
                throw new ArgumentException($"{nameof(businessTimezone)} cannot be empty");
            if (businessTimeEndUtc <= businessTimeStartUtc)
                throw new ArgumentException($"{nameof(businessTimeEndUtc)} " +
                                            $"must be greater than {nameof(businessTimeStartUtc)}");
            BusinessTimeStartUtc = businessTimeStartUtc;
            BusinessTimeEndUtc = businessTimeEndUtc;
            BusinessTimezone = businessTimezone;
        }

        public void SetBusinessEmail(string businessEmail)
        {
            if (string.IsNullOrWhiteSpace(businessEmail))
                throw new ArgumentException($"{nameof(businessEmail)} cannot be empty");
            var match = EmailRegex.Match(businessEmail);
            if (!match.Success)
                throw new ArgumentException($"{businessEmail} is not valid email.");
            BusinessEmail = businessEmail;
        }
        
        public void SetEnquiries(string[]? enquiries)
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

        private void AddResponse(Response response)
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
            if (!_responses.Contains(response))
            {
                _responses.Add(response);
            }
            _greetingResponses.Add(new GreetingResponse(this, response));
        }

        /// <summary>
        /// Publish the project by copying all its child entities such as intents,
        /// queries, responses etc. to the project of next version.
        /// </summary>
        public Project Publish(ProjectVersion latestVersion)
        {
            if (Id == Guid.Empty)
                throw new InvalidOperationException($"Project to be published must have a non-empty (GUID) Id.");
            var nextVersion = CreateWithDefaults(OrganisationId, $"{Name}__{DateTime.UtcNow.Ticks}",
                withGreetingMessage: false);

            if (_entityNames == null)
                throw new InvalidOperationException($"Load {nameof(EntityNames)} before publish.");
            if (_entityTypes == null)
                throw new InvalidOperationException($"Load {nameof(EntityTypes)} before publish.");
            if (_intents == null)
                throw new InvalidOperationException($"Load {nameof(Intents)} before publish.");
            if (_responses == null)
                throw new InvalidOperationException($"Load {nameof(Responses)} before publish.");
            if (_queries == null)
                throw new InvalidOperationException($"Load {nameof(Queries)} before publish.");
            if (_greetingResponses == null)
                throw new InvalidOperationException($"Load {nameof(GreetingResponses)} before publish.");
            
            CopyProperties(nextVersion);
            var entityNames = CopyEntityNames(nextVersion);
            var entityTypes = CopyEntityTypes(nextVersion);
            var intents = CopyIntents(nextVersion, entityNames, entityTypes);
            var responses = CopyResponses(nextVersion);
            CopyQueries(nextVersion, intents, responses);
            CopyGreetingResponses(nextVersion, responses);

            nextVersion.ProjectVersion = 
                (latestVersion ?? throw new ArgumentNullException(nameof(latestVersion))).Next(nextVersion.Id);
            
            return nextVersion;
        }

        private void CopyProperties(Project target)
        {
            target.WidgetTitle = WidgetTitle;
            target.WidgetColor = WidgetColor;
            target.WidgetDescription = WidgetDescription;
            target.FallbackMessage = FallbackMessage;
            target.Enquiries = Enquiries;
            target.Domains = Domains;
            target.BusinessTimezone = BusinessTimezone;
            target.BusinessTimeStartUtc = BusinessTimeStartUtc;
            target.BusinessTimeEndUtc = BusinessTimeEndUtc;
            target.BusinessEmail = BusinessEmail;
        }

        private Dictionary<Guid, EntityName> CopyEntityNames(Project target)
        {
            var entityNameMap = new Dictionary<Guid, EntityName>();
            foreach (var entityName in EntityNames)
            {
                var copy = new EntityName(Id, entityName.Name, entityName.CanBeReferenced);
                target.AddEntityName(copy);
                entityNameMap.Add(entityName.Id, copy);
            }

            return entityNameMap;
        }
        
        private Dictionary<Guid, EntityType> CopyEntityTypes(Project target)
        {
            var entityTypeMap = new Dictionary<Guid, EntityType>();
            foreach (var entityType in EntityTypes)
            {
                var copy = new EntityType(Id, entityType.Name, entityType.Description);
                target.AddEntityType(copy);
                entityTypeMap.Add(entityType.Id, copy);
            }

            return entityTypeMap;
        }

        private Dictionary<Guid, Intent> CopyIntents(Project target,
            Dictionary<Guid, EntityName> entityNames,
            Dictionary<Guid, EntityType> entityTypes)
        {
            var intentMap = new Dictionary<Guid, Intent>();
            foreach (var intent in Intents)
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
                target.AddIntent(copy);
            }

            return intentMap;
        }

        private Dictionary<Guid, Response> CopyResponses(Project target)
        {
            var responseMap = new Dictionary<Guid, Response>();
            foreach (var response in Responses)
            {
                var copy = new Response(Id, response.Resolution, response.Type, response.Order);
                responseMap.Add(response.Id, copy);
                target.AddResponse(copy);
            }

            return responseMap;
        }

        private void CopyQueries(Project target, IReadOnlyDictionary<Guid, Intent> intents,
            IReadOnlyDictionary<Guid, Response> responses)
        {
            foreach (var query in Queries)
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

                target.AddQuery(copy);
            }
        }

        private void CopyGreetingResponses(Project target, IReadOnlyDictionary<Guid, Response> responses)
        {
            if (target._greetingResponses == null)
                throw new InvalidOperationException($"target.{nameof(GreetingResponses)} is null");
            foreach (var greetingResponse in GreetingResponses)
            {
                var copy = new GreetingResponse(this, responses[greetingResponse.ResponseId]);
                target._greetingResponses.Add(copy);
            }
        }

        public override string ToString() => $"{Id}:{Name}";
    }
}
