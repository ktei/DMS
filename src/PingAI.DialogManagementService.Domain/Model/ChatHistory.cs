using System;
using PingAI.DialogManagementService.Domain.Utils;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ChatHistory
    {
        public Guid Id { get; private set; }
        public Guid ProjectId { get; private set; }
        public Guid SessionId { get; private set; }
        public Guid RequestId { get; private set; }
        public string? Input { get; private set; }
        public string? Output { get; private set; }
        public SessionStatus SessionStatus { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public ChatHistoryInput? ChatHistoryInput =>
            Input == null ? null : JsonUtils.TryDeserialize<ChatHistoryInput>(Input);

        public ChatHistoryOutput? ChatHistoryOutput =>
            Output == null ? null : JsonUtils.TryDeserialize<ChatHistoryOutput>(Output);

        public ChatHistory(Guid id, Guid projectId, Guid sessionId, Guid requestId,
            string? input, string? output, SessionStatus sessionStatus, DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            ProjectId = projectId;
            SessionId = sessionId;
            RequestId = requestId;
            Input = input;
            Output = output;
            SessionStatus = sessionStatus;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class ChatHistoryInput
    {
        public TextInput input { get; set; }
        private TextInput text { get; set; }
        public TextInput Text {
            get
            {
                if (this.text == null)
                {
                    return this.input;
                }

                return this.text;
            }

            set
            {
                this.text = value;
            }
        }
        
        public class TextInput
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }

    public class ChatHistoryOutput
    {
        public string Name { get; set; }
        public MatchedIntent? Intent { get; set; }

        public MatchingResult MatchingResult
        {
            get
            {
                if (Intent == null || Intent.Name == "Default Fallback Intent" || Name == "NluLowConfidence" || Name == "NoIntent")
                {
                    return MatchingResult.MissingIntent;
                }

                if (Name == "NoMatchingQueries")
                {
                    return MatchingResult.MissingQuery;
                }

                return MatchingResult.Matched;
            }
        }
        public class MatchedIntent
        {
            public string Name { get; set; }
        }
    }
}
