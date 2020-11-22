using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Utils;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Reporting.GenerateHtmlReport
{
    public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, Report>
    {
        private readonly IChatHistoryRepository _chatHistoryRepository;
        private readonly IAuthorizationService _authorizationService;


        public GenerateReportCommandHandler(IChatHistoryRepository chatHistoryRepository,
            IAuthorizationService authorizationService)
        {
            _chatHistoryRepository = chatHistoryRepository;
            _authorizationService = authorizationService;
        }

        public async Task<Report> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
        {
            var canReadProject = await _authorizationService.UserCanReadProject(request.DesignTimeProjectId);
            if (!canReadProject)
                throw new ForbiddenException(ErrorDescriptions.ProjectReadDenied);

            var sydneyLocalTime = DateTime.UtcNow.ConvertToLocal("Australia/Sydney");
            var sydneyEndOfToday = sydneyLocalTime.Date.AddDays(1).AddSeconds(-1); // end of today local
            var chatHistories = await _chatHistoryRepository.GetChatHistories(request.DesignTimeProjectId,
                request.TimeRangeStartUtc, sydneyEndOfToday.ToUniversalTime());

            var report = new Report("Dialog report since " +
                                    $"{request.TimeRangeStartUtc.ConvertToLocal("Australia/Sydney"):yyyy MMMM dd}");
            report.Build(chatHistories);

            return report;
        }
    }
}