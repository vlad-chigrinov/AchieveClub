using AchieveClub.Server.RepositoryItems;
using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompletedAchievementsController(ApplicationContext db) : ControllerBase
    {
        private ApplicationContext _db = db;

        [Authorize]
        [HttpGet]
        public ActionResult<List<CompletedAchievementState>> GetByUserId()
        {
            var cookie = Request.Cookies["X-User-Id"];
            if (cookie == null || int.TryParse(cookie, out int userId) == false)
                return BadRequest("User not found!");

            if (_db.Users.Count(x => x.Id == userId) == 0)
                return BadRequest("User not found!");

            return _db.CompletedAchievements.Where(ca=>ca.UserRefId == userId).Select(ca=>new CompletedAchievementState(ca.AchieveRefId)).ToList();
        }
    }
}
