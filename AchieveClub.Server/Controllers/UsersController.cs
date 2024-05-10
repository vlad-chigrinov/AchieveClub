using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(ApplicationContext db, UserStatisticsService userStatistics) : ControllerBase
    {
        private ApplicationContext _db = db;
        private UserStatisticsService _userStatistics = userStatistics;

        [Authorize]
        [HttpGet("current")]
        public ActionResult<UserState> GetCurrent()
        {
            var cookie = Request.Cookies["X-User-Id"];
            if (cookie == null || int.TryParse(cookie, out int userId) == false)
                return BadRequest("User not found!");

            var result = _db.Users.Include(u => u.Club).FirstOrDefault(u => u.Id == userId);
            if (result == null)
            {
                return BadRequest("User not found!");
            }
            else
            {
                return result.ToUserState(_userStatistics.GetXpSumById(result.Id));
            }
        }

        [HttpGet("{userId}")]
        public ActionResult<UserState> GetById([FromRoute] int userId)
        {
            var result = _db.Users.Include(u => u.Club).FirstOrDefault(u => u.Id == userId);
            if (result == null)
            {
                return BadRequest("User not found!");
            }
            else
            {
                return result.ToUserState(_userStatistics.GetXpSumById(result.Id));
            }
        }

        [HttpGet]
        public ActionResult<List<UserState>> GetAll()
        {
            return _db.Users
                .Include(u => u.Club)
                .ToList()
                .Select(u => u.ToUserState(_userStatistics.GetXpSumById(u.Id)))
                .ToList();
        }
    }
}