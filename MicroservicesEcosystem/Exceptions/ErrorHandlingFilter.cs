using System;
using System.Net;
using MicroservicesEcosystem.CustomDataTime;
using MicroservicesEcosystem.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MicroservicesEcosystem.Exceptions
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        public ErrorHandlingFilter(ILogger<ErrorHandlingFilter> logger)
        {
            this._logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "MS_Sales_Document");
            HandleExceptionAsync(context);
            context.ExceptionHandled = true;
        }
        private void HandleExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;
            if (exception is ArgumentException)
                SetExceptionResult(context, exception, HttpStatusCode.BadRequest);
            else if (exception is UnauthorizedAccessException)
                SetExceptionResult(context, exception, HttpStatusCode.Unauthorized);
            else
               SetExceptionResult(context, exception, HttpStatusCode.InternalServerError);
        }
        private async void SetExceptionResult(ExceptionContext context, Exception exception, HttpStatusCode code)
        {
            context.Result = new JsonResult(new ApiResponse(exception, code))
            {
                StatusCode = (int)code               
            };
        }
    }
}
