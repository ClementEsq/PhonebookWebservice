using System.Data.Entity;
using System.Net;
using System.Web.Http;
using Autofac;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(Phonebook.Webservice.Startup))]
namespace Phonebook.Webservice
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var config = new HttpConfiguration();

            JsonConfig.Configuration(config);
            var container = AutofacConfig.Configuration(config);
            WebApiConfig.Register(config);
            StatsDConfig.Configuration();
            

           
            app.Use<LoggingMiddleware>();
            app.UseAutofacMiddleware(container);
            app.UseWebApi(config);
        }
    }

}
