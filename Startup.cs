using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Online_Shopping_Website.Startup))]
namespace Online_Shopping_Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
