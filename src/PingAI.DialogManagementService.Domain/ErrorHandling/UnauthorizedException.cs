namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message) : base(401, message, null)
        {
        }
        
        public UnauthorizedException(string message, string errorCode) : base(401, message, errorCode)
        {
        }
    }
}