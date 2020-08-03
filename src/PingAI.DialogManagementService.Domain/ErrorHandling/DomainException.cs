using System;

namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class DomainException : Exception
    {
        public int StatusCode { get; private set; }
        
        public DomainException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}