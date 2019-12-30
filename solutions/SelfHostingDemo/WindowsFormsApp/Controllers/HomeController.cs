using System.Web.Http;

namespace WindowsFormsApp.Controllers
{
    /// <summary>
    /// Home controller.
    /// </summary>
    [RoutePrefix("api/home")]
    public class HomeController : ApiController
    {
        /// <summary>
        /// Print the greetings
        /// </summary>
        /// <param name="name">visitor</param>
        /// <returns>greetings</returns>
        [Route("echo")]
        [HttpGet]
        public IHttpActionResult Echo(string name)
        {
            return Json(new {Name = name, Message = $"Hello, {name}"});
        }
    }
}
