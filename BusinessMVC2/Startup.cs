using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BusinessMVC2.Startup))]
namespace BusinessMVC2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
