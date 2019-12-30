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
        private HttpSelfHostServer _server;

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

            this._server = new HttpSelfHostServer(config);
            this._server.OpenAsync().Wait();
        }

        /// <summary>
        /// Close HTTP server.
        /// </summary>
        public void CloseHttpServer()
        {
            this._server.CloseAsync().Wait();
            this._server.Dispose();
        }

        #endregion
    }
}
