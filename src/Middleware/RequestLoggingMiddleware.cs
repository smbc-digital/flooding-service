using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace flooding_service.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            // Leave the body open so the next middleware can read it.
            using (var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                // Do some processing with body…
                _logger.LogWarning($"***DEBUG:: Received request with body {body}");
                // Reset the request body stream position so the next middleware can read it
                context.Request.Body.Position = 0;
            }

            await _next(context);
        }
    }
}
