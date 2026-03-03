using System;
using System.Linq;
using System.Net;
using MicroservicesEcosystem.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MicroservicesEcosystem.Exceptions
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ErrorHandlingFilter> _logger;

        public ErrorHandlingFilter(ILogger<ErrorHandlingFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;

            string route = request.Path;
            string method = request.Method;
            string traceId = httpContext.TraceIdentifier;
            string correlationId = request.Headers["X-Correlation-Id"].FirstOrDefault() ?? "N/A";
            string user = httpContext.User?.Identity?.Name ?? "Anonymous";
            string tenant = request.Headers["X-Tenant"].FirstOrDefault() ?? "N/A";
            string ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            HandleExceptionAsync(
                context,
                route,
                method,
                user,
                tenant,
                ip,
                traceId,
                correlationId
            );

            context.ExceptionHandled = true;
        }

        private void HandleExceptionAsync(
            ExceptionContext context,
            string route,
            string method,
            string user,
            string tenant,
            string ip,
            string traceId,
            string correlationId)
        {
            var exception = context.Exception;

            if (exception is ArgumentException)
            {
                SetExceptionResult(context, exception, HttpStatusCode.BadRequest, traceId);
                return;
            }

            if (exception is UnauthorizedAccessException)
            {
                SetExceptionResult(context, exception, HttpStatusCode.Unauthorized, traceId);
                return;
            }

            // 🔴 SOLO LOS 500 GENERAN CÓDIGO
            string errorCode = GenerateShortErrorCode();

            _logger.LogError(exception,
                "ERROR 500 | Code:{ErrorCode} | Service:{Service} | Endpoint:{Endpoint} | Method:{Method} | User:{User} | Tenant:{Tenant} | IP:{IP} | TraceId:{TraceId} | CorrelationId:{CorrelationId}",
                errorCode,
                "MS_Electronic_Service",
                route,
                method,
                user,
                tenant,
                ip,
                traceId,
                correlationId
            );

            SetInternalServerError(context, traceId, errorCode);
        }

        private static void SetExceptionResult(
            ExceptionContext context,
            Exception exception,
            HttpStatusCode code,
            string traceId)
        {
            var response = new ApiResponse(exception, code);
            response.TraceId = traceId;
            response.Path = context.HttpContext.Request.Path;

            context.Result = new JsonResult(response)
            {
                StatusCode = (int)code
            };
        }

        private static void SetInternalServerError(
            ExceptionContext context,
            string traceId,
            string errorCode)
        {
            var response = new ApiResponse(
                new Exception("Ha ocurrido un error interno. Comuniquese con el area de TI e indique el codigo proporcionado."),
                HttpStatusCode.InternalServerError
            );

            response.TraceId = traceId;
            response.Path = context.HttpContext.Request.Path;

            // Agregamos código al mensaje
            response.Message = $"[{errorCode}] Ha ocurrido un error interno. Comuniquese con el area de TI e indique el codigo proporcionado.";

            context.Result = new JsonResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        private static string GenerateShortErrorCode()
        {
            return "ERR-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
        }
    }
}