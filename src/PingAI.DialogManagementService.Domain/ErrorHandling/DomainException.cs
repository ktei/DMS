using System;

namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class DomainException : Exception
    {
        public int StatusCode { get; private set; }
        public string? ErrorCode { get; private set; }
        
        public DomainException(int statusCode, string message, string? errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}