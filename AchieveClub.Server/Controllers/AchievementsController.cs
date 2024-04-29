using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController(ApplicationContext db) : ControllerBase
    {
        private ApplicationContext _db = db;

        [HttpGet]
        public ActionResult<List<AchievementDbo>> GetAll()
        {
            return _db.Achievements.ToList();
        }
    }
}
