using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Phonebook.Webservice.Filters;
using Phonebook.Webservice.Models.Connections;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.RollingFileAlternate;
using Phonebook.Webservice.Infrastructure;
using Phonebook.Webservice.Infrastructure.Repositories;

namespace Phonebook.Webservice
{
    public class AutofacConfig
    {
        public static IContainer Configuration(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            ConfigureSerilog(builder);

            ConfigureWebApi(builder, config);
            ConfigureDatabaseConnections(builder);
            ConfigureDataRepositories(builder);
            ConfigureServices(builder);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return container;
        }


        public static void ConfigureDatabaseConnections(ContainerBuilder builder)
        {
            #region Db.Builder
            builder.Register(i => new SqlConnection(ConfigurationManager.ConnectionStrings["PhoneBookDb"].ConnectionString))
                  .Named<IDbConnection>("PhoneBookDb")
                  .InstancePerRequest()
                  .InstancePerLifetimeScope();

            builder.Register(ctx => new PhonebookDbConnection(ctx.ResolveNamed<IDbConnection>("PhoneBookDb")))
                   .AsImplementedInterfaces()
                   .InstancePerRequest()
                   .InstancePerLifetimeScope();
            #endregion
        }

        public static void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<PhonebookService>().AsImplementedInterfaces();
        }

        public static void ConfigureDataRepositories(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(ServiceClientRepository)).AsImplementedInterfaces();
        }

        public static void ConfigureWebApi(ContainerBuilder builder, HttpConfiguration config)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterType<BasicPasswordAuthenticationFilter>()
                   .AsWebApiActionFilterFor<ApiController>()
                   .InstancePerRequest();
            builder.RegisterType<ExceptionFilter>()
                   .AsWebApiActionFilterFor<ApiController>()
                   .InstancePerRequest();
        }

        public static void ConfigureSerilog(ContainerBuilder builder)
        {
            builder.Register(i => new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.RollingFileAlternate("..\\logs", LogEventLevel.Warning)
                    .CreateLogger())
                    .As<ILogger>()
                   .SingleInstance();
        }
    }
}
