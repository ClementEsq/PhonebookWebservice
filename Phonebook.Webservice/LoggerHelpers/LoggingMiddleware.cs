using Microsoft.Owin;
using StatsdClient;
using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.RollingFileAlternate;

namespace Phonebook.Webservice
{
    public class LoggingMiddleware : OwinMiddleware
    {
        public LoggingMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            await Next.Invoke(context);

            stopwatch.Stop();

            var date = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            var client = context.Request.Headers.Get("X-Forwarded-For") ?? context.Request.RemoteIpAddress;
            var method = context.Request.Method;
            var path = context.Request.Uri.PathAndQuery;
            var responseTime = stopwatch.ElapsedMilliseconds();

            DogStatsd.Increment("ActionExecuted");
            DogStatsd.Increment($"ActionExecuted.StatusCode.{context.Response.StatusCode}_{context.Response.ReasonPhrase}");
            DogStatsd.Histogram("ActionExecuted.ResponseTime", responseTime);

            var message = $"{date} {client} \"{method} {path}\" {context.Response.StatusCode} {responseTime}";

#if !RELEASE
            Console.WriteLine(message);
#endif
            logger.Information(message);
        }

        private static ILogger logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.RollingFileAlternate("..\\logs\\access", outputTemplate: "{Message}{NewLine}")
            .CreateLogger();
    }
}