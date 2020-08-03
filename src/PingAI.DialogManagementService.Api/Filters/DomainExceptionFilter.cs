using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using PingAI.DialogManagementService.Api.Models;
using PingAI.DialogManagementService.Domain.ErrorHandling;

namespace PingAI.DialogManagementService.Api.Filters
{
    public class DomainExceptionFilter : IActionFilter, IOrderedFilter
    {
        private readonly IWebHostEnvironment _environment;

        public DomainExceptionFilter(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is null) return;
            
            if (context.Exception is DomainException domainException)
            {
                context.Result = new ObjectResult(new ErrorsDto(domainException.Message))
                {
                    StatusCode = domainException.StatusCode
                };
                context.ExceptionHandled = true;
                return;
            }

            if (_environment.IsProduction())
            {
                context.Result = new ObjectResult(new ErrorsDto("Oops! Sever has encountered an error. Please try again later."))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            else
            {
                context.Result = new ObjectResult(new ErrorsDto(context.Exception.ToString()))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            context.ExceptionHandled = true;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public int Order { get; set; } = int.MaxValue - 10;    
    }
}