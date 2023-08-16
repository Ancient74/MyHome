using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHomeLib;

namespace MyHomeWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesktopController : ControllerBase
    {
        private IMyHomeApiService myHomeApiService;

        public DesktopController(IMyHomeApiService myHomeApiService)
        {
            this.myHomeApiService = myHomeApiService;
        }

        [Route("PCShutdown")]
        [HttpPost]
        public IActionResult PCShutdown([FromBody]ShutdownMode shutdownMode)
        {
            if (shutdownMode == ShutdownMode.Undefined)
                return BadRequest();

            myHomeApiService.GetDesktopManager().Shutdown(shutdownMode);
            return Ok();
        }

        [Route("Ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok();
        }

        [Route("OpenInBrowser")]
        [HttpPost]
        public IActionResult OpenInBrowser([FromBody]string url)
        {
            if (url.Length == 0)
                return BadRequest();

            myHomeApiService.GetDesktopManager().OpenInBrowser(url);
            return Ok();
        }
    }
}
