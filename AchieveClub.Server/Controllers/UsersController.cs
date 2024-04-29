using AchieveClub.Server.RepositoryItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(ApplicationContext db) : ControllerBase
    {
        private ApplicationContext _db = db;

        [Authorize]
        [HttpGet()]
        public ActionResult<UserDbo> GetById()
        {
            var cookie = Request.Cookies["X-User-Id"];
            if (cookie == null || int.TryParse(cookie, out int userId) == false)
                return BadRequest("User not found!");

            var result = _db.Users.Include(u=>u.Club).FirstOrDefault(u => u.Id == userId);
            if (result == null)
            {
                return BadRequest("User not found!");
            }
            else
            {
                return Ok(result.ToUserState());
            }
        }
    }
}