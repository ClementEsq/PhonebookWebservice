using Serilog;
using Serilog.Sinks.RollingFileAlternate;
using System;
using StatsdClient;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Phonebook.Webservice
{
    public class LoggingDelegatingHandler : DelegatingHandler
    {
        string name;
        ILogger logger;

        public LoggingDelegatingHandler(string name) : base(new HttpClientHandler())
        {
            this.name = name;
            this.logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.RollingFileAlternate($"..\\logs\\HttpClient\\{name}", outputTemplate: "{Message}{NewLine}")
                .CreateLogger();
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = await base.SendAsync(request, cancellationToken);

            var date = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            var responseTime = stopwatch.ElapsedMilliseconds();

            DogStatsd.Increment($"HttpClient.{this.name}");
            DogStatsd.Increment($"HttpClient.{this.name}.StatusCode.{response.StatusCode}_{response.ReasonPhrase}");
            DogStatsd.Histogram($"HttpClient.{this.name}.ResponseTime", responseTime);

            var message = $"{date} {request.Method} {request.RequestUri} => {response.StatusCode} {responseTime}";

#if DEBUG
            System.Console.WriteLine($"{date} HttpClient[{this.name}] {request.Method} {request.RequestUri} => {response.StatusCode} {responseTime}");
#endif

            this.logger.Information(message);

            return response;
        }
    }
}
