using System;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Microsoft.Owin.Hosting;
using Serilog;
using Phonebook.Webservice;

namespace Phonebook.Webservice.Service
{
    public class PhonebookService
    {
        private IDisposable app;
        private string port = string.Empty;
        public void Start()
        {
#if DEBUG
            port = ConfigurationManager.AppSettings["HostPortDebug"];
            app = WebApp.Start<Startup>($"http://localhost:{port}");
#else
            port = ConfigurationManager.AppSettings["HostPortProduction"];
            app = WebApp.Start<Startup>($"http://*:{port}");
#endif

            var container = AutofacConfig.Configuration(new HttpConfiguration());

            

        }

      
        public void Stop()
        {
            app.Dispose();
        }
    }
}
