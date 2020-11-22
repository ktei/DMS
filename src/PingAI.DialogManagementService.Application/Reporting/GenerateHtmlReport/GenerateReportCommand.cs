using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Reporting.GenerateHtmlReport
{
    public class GenerateReportCommand : IRequest<Report>
    {
        public Guid DesignTimeProjectId { get; set; }
        public DateTime TimeRangeStartUtc { get; set; }

        public GenerateReportCommand(Guid designTimeProjectId, DateTime timeRangeStartUtc)
        {
            DesignTimeProjectId = designTimeProjectId;
            TimeRangeStartUtc = timeRangeStartUtc;
        }

        public GenerateReportCommand()
        {
            
        }
    }
}
