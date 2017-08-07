using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using Phonebook.Webservice.Contracts;
using Phonebook.Webservice.Extensions;
using Phonebook.Webservice.Contracts.Requests_and_Responses.Responses;
using Phonebook.Webservice.Controllers;
using System.Linq;
using Phonebook.Webservice.Models;
using Phonebook.Webservice.Infrastructure.Interfaces;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Filters
{
    public class BasicPasswordAuthenticationFilter : IAutofacActionFilter
    {
        private readonly IRepository<ServiceClient> ServiceClientRepository;

        public BasicPasswordAuthenticationFilter(IRepository<ServiceClient> ServiceClientRepository)
        {
            this.ServiceClientRepository = ServiceClientRepository;
        }

        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {

            BasicAuthenticationIdentity identity;
            identity = ParseAuthorizationHeader(actionContext);

            if (identity == null)
            {
                Challenge(actionContext);
                return Task.FromResult(0);
            }

            if (!OnAuthorizeUser(identity.Name, identity.password, actionContext))
            {
                    Challenge(actionContext);
                    return Task.FromResult(0);
            }
            

            var principal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = principal;
            actionContext.RequestContext.Principal = principal;

            return Task.FromResult(0);
        }

        protected virtual bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            var client = ServiceClientRepository.GetByIdentifier(username);

            return client != null && password.Equals(client.Password.Trim(), StringComparison.InvariantCulture);
        }

        protected BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            var auth = actionContext.Request.Headers.Authorization;
            if (auth == null || !auth.Scheme.Equals("basic", StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrWhiteSpace(auth.Parameter))
            {
                return null;
            }

            var authHeader = Encoding.Default.GetString(Convert.FromBase64String(auth.Parameter));

            var tokens = authHeader.Split(':');

            return tokens.Length < 2 ? null : new BasicAuthenticationIdentity(tokens[0].Trim(), tokens[1].Trim());
        }

        private void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"{host}\"");

            var result = GenericApiResponse<object>.Failure();
            result.Message = "";
            result.Payload = new { };

            actionContext.Response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
        }

        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
