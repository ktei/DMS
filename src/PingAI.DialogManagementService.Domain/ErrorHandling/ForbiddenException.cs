namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class ForbiddenException : DomainException
    {
        public ForbiddenException(string message) : base(403, message, null)
        {
        }

        public ForbiddenException(string message, string errorCode) : base(403, message, errorCode)
        {
        }
    }
}