using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.Shared
{
    public class Response
    {
        public string? RteText { get; }
        public FormResolution? Form { get; }
        public int Order { get; }

        public Response(string? rteText, FormResolution? form, int order)
        {
            RteText = rteText;
            Form = form;
            Order = order;
        }
    }
}