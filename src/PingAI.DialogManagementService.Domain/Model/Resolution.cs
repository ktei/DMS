using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using PingAI.DialogManagementService.Domain.ErrorHandling;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Resolution
    {
        [JsonPropertyName("type")]
        public ResolutionType Type { get; }

        [JsonPropertyName("parts")] public ResolutionPart[]? Parts { get; }

        [JsonPropertyName("form")] public FormResolution? Form { get; }

        [JsonConstructor]
        public Resolution(ResolutionType type, IEnumerable<ResolutionPart>? parts, FormResolution? form)
        {
            Type = type;
            Parts = parts?.ToArray();
            Form = form;
        }

        public static Resolution Empty => new Resolution(ResolutionType.EMPTY, null, null);
        
        public const int MaxRteTextLength = 4000;

        public class Factory
        {
            public static Resolution RteText(string rteText)
            {
                return RteText(rteText, new Dictionary<string, EntityName>());
            }

            /// <summary>
            /// Constructs a Resolution from RTE and parses the raw text
            /// into an array of <see cref="ResolutionPart"/>
            /// </summary>
            /// <param name="rteText">RTE text that will be parsed to array of <see cref="ResolutionPart"/></param>
            /// <param name="entityNames">A map of all entity names of current project, the key being the name</param>
            public static Resolution RteText(string rteText, IDictionary<string, EntityName> entityNames)
            {

                if (string.IsNullOrEmpty(rteText))
                    throw new ArgumentException($"{nameof(rteText)} cannot be empty");

                if (rteText.Length > MaxRteTextLength)
                    throw new ArgumentException($"Max length of {nameof(rteText)} is {MaxRteTextLength}");

                var resolutionParts = new List<ResolutionPart>();
                var re = new Regex(@"\$\{([A-Za-z-_0-9]+)\}");
                var pos = 0;
                Match m;
                do
                {
                    m = re.Match(rteText, pos);
                    if (!m.Success) break;

                    if (m.Index > pos)
                    {
                        // add plain text before current param
                        resolutionParts.Add(ResolutionPart.TextPart(rteText.Substring(pos, m.Index - pos)));
                    }

                    // add current param
                    if (entityNames.TryGetValue(m.Groups[1].Value, out var entityName))
                    {
                        resolutionParts.Add(ResolutionPart.EntityNamePart(entityName.Id));
                    }
                    else
                    {
                        throw new BadRequestException(
                            string.Format(ErrorDescriptions.EntityNameNotFound, m.Groups[1].Value));
                    }

                    // move the cursor to the end of the current param
                    pos = m.Index + m.Groups[0].Length;
                } while (m!.Success);

                // add the rest of the plain text
                if (rteText.Length > pos)
                {
                    resolutionParts.Add(ResolutionPart.TextPart(rteText[pos..]));
                }

                return new Resolution(ResolutionType.PARTS, resolutionParts, null);
            }

            public static Resolution Form(FormResolution form)
            {
                return new Resolution(ResolutionType.PARTS, null,
                    form ?? throw new ArgumentNullException(nameof(form)));
            }
        }
    }
}
