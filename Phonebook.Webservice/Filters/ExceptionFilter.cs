using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Serilog;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Filters
{
    public class ExceptionFilter : IAutofacActionFilter
    {
        private readonly ILogger logger;

        public ExceptionFilter(ILogger logger)
        {
            this.logger = logger;
        }

        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
#if RELEASE
            if (actionExecutedContext.Exception == null)
            {
                return;
            }

            logger.Fatal(actionExecutedContext.Exception, "Unhandled exception");

            var obj = ((ApiController)actionExecutedContext.ActionContext.ControllerContext.Controller).GetResponse((object)null);

            actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            };

            actionExecutedContext.Exception = null;
#endif
            return Task.FromResult(0);
        }
    }
}