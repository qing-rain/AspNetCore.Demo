using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QingRain.Api.Controllers
{
    [Route("test")]
    [Authorize]
    public class TestController : ControllerBase
    {
        [Authorize(Policy = "mygroup")]
        // [Authorize(Roles = "qingrainrole")]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
