using System;

namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
            
        }
    }
}