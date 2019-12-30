using System.Web.Http;
using System.Web.Http.SelfHost;

namespace WindowsFormsApp.Services
{
    /// <summary>
    /// HTTP service.
    /// </summary>
    public class HttpService
    {
        /// <summary>
        /// HTTP self hosting.
        /// </summary>
        protected HttpSelfHostServer HostServer { get; set; }

        #region HTTP Service

        /// <summary>
        /// start HTTP server.
        /// </summary>
        /// <param name="port">the port of the http server</param>
        public void StartHttpServer(string port)
        {
            var config = new HttpSelfHostConfiguration($"http://0.0.0.0:{port}");

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}");

            this.HostServer = new HttpSelfHostServer(config);
            this.HostServer.OpenAsync().Wait();
        }

        /// <summary>
        /// Close HTTP server.
        /// </summary>
        public void CloseHttpServer()
        {
            this.HostServer.CloseAsync().Wait();
            this.HostServer.Dispose();
        }

        #endregion
    }
}
