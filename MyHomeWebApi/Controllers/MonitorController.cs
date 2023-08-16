using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHomeLib;

namespace MyHomeWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        private IMyHomeApiService myHomeApiService;

        public MonitorController(IMyHomeApiService myHomeApiService)
        {
            this.myHomeApiService = myHomeApiService;
        }

        [Route("OpenBigPicture")]
        [HttpPost]
        public IActionResult OpenBigPicture()
        {
            var monitorManager = myHomeApiService.GetMonitorManager();
            monitorManager.OpenBigPicture();
            return Ok();
        }

        [Route("MonitorMode")]
        [HttpPost]
        public IActionResult SetMonitorMode([FromBody]MonitorMode monitorMode)
        {
            if (monitorMode == MonitorMode.Undefined)
                return BadRequest();
            var monitorManager = myHomeApiService.GetMonitorManager();
            monitorManager.SetMonitorMode(monitorMode);
            return Ok();
        }
    }
}
