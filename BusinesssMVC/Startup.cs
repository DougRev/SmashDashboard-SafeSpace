using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BusinesssMVC.Startup))]
namespace BusinesssMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
