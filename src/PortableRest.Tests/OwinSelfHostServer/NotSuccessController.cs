using System.Web.Http;

namespace PortableRest.Tests.OwinSelfHostServer
{
    [RoutePrefix("notsuccess")]
    public class NotSuccessController : ApiController
    {
        /// <summary>
        /// Returns a 500 (Internal Server Error) response.
        /// </summary>
        /// <returns></returns>
        [Route("internalservererror")]
        [HttpGet]
        public IHttpActionResult GetInternalServerError()
        {
            return InternalServerError();
        }

        /// <summary>
        /// Returns a 404 (Not Found) response.
        /// </summary>
        [Route("notfound")]
        [HttpGet]
        public IHttpActionResult GetNotFound()
        {
            return NotFound();
        }
    }
}