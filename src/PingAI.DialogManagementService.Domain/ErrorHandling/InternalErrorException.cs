namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class InternalErrorException : DomainException
    {
        public InternalErrorException(string message) : base(500, message, null)
        {
        }

        public InternalErrorException(string message, string errorCode) : base(500, message, errorCode)
        {
        }
    }
}