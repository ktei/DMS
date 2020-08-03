namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class InternalErrorException : DomainException
    {
        public InternalErrorException(string message) : base(500, message)
        {
        }
    }
}