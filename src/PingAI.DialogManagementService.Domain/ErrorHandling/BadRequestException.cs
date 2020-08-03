namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class BadRequestException : DomainException
    {
        public BadRequestException(string message) : base(400, message)
        {
        }
    }
}