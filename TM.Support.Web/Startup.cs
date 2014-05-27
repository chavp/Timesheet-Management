using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PJ_CWN019.TM.Support.Web.Startup))]
namespace PJ_CWN019.TM.Support.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}