using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CyberPark.Website.Startup))]
namespace CyberPark.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
