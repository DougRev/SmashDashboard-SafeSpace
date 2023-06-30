using Autofac;
using Autofac.Integration.Mvc;
using BusinessData;
using BusinessData.Interfaces;
using BusinessMVC2.Controllers;
using BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BusinessMVC2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           
           var builder = new ContainerBuilder();

            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Register dependencies in filter attributes.
            builder.RegisterFilterProvider();

            // Register dependencies in custom views.
            builder.RegisterSource(new ViewRegistrationSource());

            // Register your service types.
            builder.RegisterType<ApplicationDbContext>().InstancePerRequest();
            builder.RegisterType<FranchiseService>().InstancePerRequest();
            builder.RegisterType<UserIdProvider>().As<IUserIdProvider>().InstancePerLifetimeScope();
            builder.RegisterType<NationalAccountService>().AsSelf();
            builder.RegisterType<ClientService>().AsSelf();


            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Other configuration...
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
