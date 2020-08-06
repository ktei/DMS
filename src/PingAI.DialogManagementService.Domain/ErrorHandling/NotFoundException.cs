namespace PingAI.DialogManagementService.Domain.ErrorHandling
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(400, message)
        {
        }
    }
}