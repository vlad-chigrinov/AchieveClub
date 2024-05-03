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

        [Authorize(Roles = "Student")]
        [HttpGet("Student")]
        public ActionResult<string> GetByStudent()
        {
            return Ok("pong");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin")]
        public ActionResult<string> GetByAdmin()
        {
            return Ok("pong");
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet("Supervisor")]
        public ActionResult<string> GetBySupervisor()
        {
            return Ok("pong");
        }

        [Authorize(Roles = "TestUser")]
        [HttpGet("TestUser")]
        public ActionResult<string> GetByTestUser()
        {
            return Ok("pong");
        }
    }
}