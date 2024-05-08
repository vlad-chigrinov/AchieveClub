using AchieveClub.Server.Services;
using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController(ApplicationContext db, AchievementStatisticsSevice achievementStatistics) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly AchievementStatisticsSevice _achievementStatistics = achievementStatistics;

        [HttpGet]
        public ActionResult<List<AchievementState>> GetAll()
        {
            return _db.Achievements.ToList().Select(a => a.ToState(_achievementStatistics.GetCompletionRatioById(a.Id))).ToList();
        }
    }
}
