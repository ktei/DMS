using System;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Application.Reporting.GenerateHtmlReport;
using PingAI.DialogManagementService.Application.Utils;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiVersion("1")]
    [Authorize]
    public class ReportsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ContentResult> GenerateReport([FromQuery] Guid? projectId,
            [FromQuery] int? days)
        {
            if (!projectId.HasValue)
                throw new BadRequestException($"{nameof(projectId)} must not be empty.");
            if (!days.HasValue)
                throw new BadRequestException($"{nameof(days)} must not be empty.");
            if (days <= 0)
                throw new BadRequestException($"{nameof(days)} must be greater than 0.");
            if (days > 90)
                throw new BadRequestException($"{nameof(days)} must be less or equal to 90");

            var sydneyLocalTimeNow = DateTime.UtcNow.ConvertToLocal("Australia/Sydney");
            var sydneyLocalTimeStart = sydneyLocalTimeNow.Date.AddDays(-days.Value);
            var report = await _mediator.Send(new GenerateReportCommand(projectId.Value,
                sydneyLocalTimeStart.ToUniversalTime()));

            var reportHtml = GenerateReportHtml(report);
            return Content(reportHtml, "text/plain");
        }

        private static string GenerateReportHtml(Report report)
        {
            var sb = new StringBuilder();
            
            // Unmatched phrases
            
            sb.Append("<div>");
            sb.Append("<h2 class=\"unmatched-phrases\">Unmatched phrases</h2>");
            sb.Append("<table>");

            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<td>User phrases</td>");
            sb.Append("<td>Error reason</td>");
            sb.Append("<td>Timestamp</td>");
            sb.Append("</tr>");
            sb.Append("</thead>");

            sb.Append("<tbody>");
            foreach (var unmatchedPhrase in report.UnmatchedPhrases)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{unmatchedPhrase.Phrase}</td>");
                sb.Append($"<td>{unmatchedPhrase.Reason}</td>");
                sb.Append($"<td>{unmatchedPhrase.Timestamp.ConvertToLocal("Australia/Sydney"):G}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody>");

            sb.Append("</table>");
            sb.Append("</div>");
            
            sb.Append("<br />");
            
            // Dialogs
            
            sb.Append("<div>");
            sb.Append("<h2>History dialogs</h2>");
            sb.Append("<table>");

            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<td>User phrases</td>");
            sb.Append("<td>Matched FAQ</td>");
            sb.Append("<td>Matching result</td>");
            sb.Append("<td>Timestamp</td>");
            sb.Append("</tr>");
            sb.Append("</thead>");

            sb.Append("<tbody>");
            foreach (var dialog in report.Dialogs)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{dialog.UserPhrases}</td>");
                sb.Append($"<td>{dialog.MatchedFaq}</td>");
                sb.Append($"<td>{dialog.MatchingResult}</td>");
                sb.Append($"<td>{dialog.Timestamp.ConvertToLocal("Australia/Sydney"):G}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody>");

            sb.Append("</table>");
            sb.Append("</div>");

            return sb.ToString();
        }
    }
}
