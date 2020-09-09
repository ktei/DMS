namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(400, message, null)
        {
        }
        
        public NotFoundException(string message, string errorCode) : base(400, message, errorCode)
        {
        }

    }
}