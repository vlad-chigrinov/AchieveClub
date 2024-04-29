using AchieveClub.Server.RepositoryItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {

        [Authorize]
        [HttpGet()]
        public ActionResult<string> Get()
        {
            return Ok("pong");
        }
    }
}