using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AccountPlugin.Core.Startup))]
namespace AccountPlugin.Core
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
