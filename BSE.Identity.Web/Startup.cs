using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BSE.Identity.Web.Startup))]
namespace BSE.Identity.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
