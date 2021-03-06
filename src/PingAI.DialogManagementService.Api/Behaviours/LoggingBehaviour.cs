using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PingAI.DialogManagementService.Api.Behaviours
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var requestId = Guid.NewGuid();
            using var _ = _logger.BeginScope("Received new request {requestId}", requestId);
            _logger.LogDebug($"Handling {typeof(TRequest).Name} {{@request}}", request);
            var response = await next();
            _logger.LogDebug($"Handled {typeof(TRequest).Name}");

            return response;
        }
    }
}