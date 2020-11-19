using System;

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
    
    public enum SessionStatus
    {
        CHATBOT,
        HANDOVER
    }
}
