using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.ErrorHandling;

namespace PingAI.DialogManagementService.Infrastructure.Utils
{
    internal static class HttpResponseMessageExtensions
    {
        public static async Task<DomainException> AsDomainException(this HttpResponseMessage httpResponse)
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Could not cast a successful response to exception");
            }

            var message = await httpResponse.Content.ReadAsStringAsync();
            return httpResponse.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestException(message),
                HttpStatusCode.Forbidden => new ForbiddenException(message),
                HttpStatusCode.Unauthorized => new UnauthorizedException(message),
                HttpStatusCode.NotFound => new NotFoundException(message),
                HttpStatusCode.InternalServerError => new InternalErrorException(message),
                _ => new DomainException((int)httpResponse.StatusCode, message)
            };
        }
    }
}