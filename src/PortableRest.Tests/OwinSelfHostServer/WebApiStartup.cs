using System.Web.Http;
using Owin;

namespace PortableRest.Tests.OwinSelfHostServer
{
    public class WebApiStartup
    {
        /// <summary>
        /// This code configures Web API. The WebApiStartup class is specified as a type
        /// parameter in the WebApp.Start method.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }
}