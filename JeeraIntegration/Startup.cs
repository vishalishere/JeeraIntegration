using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JiraIntegration.Startup))]
namespace JiraIntegration
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
