using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MicroservicesEcosystem.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey(HeaderName))
            {
                context.Request.Headers[HeaderName] = Guid.NewGuid().ToString();
            }

            context.Response.Headers[HeaderName] =
                context.Request.Headers[HeaderName];

            await _next(context);
        }
    }
}