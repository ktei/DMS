namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class BadRequestException : DomainException
    {
        public BadRequestException(string message) : base(400, message, null)
        {
        }
        
        public BadRequestException(string message, string errorCode) : base(400, message, errorCode)
        {
        }

    }
}