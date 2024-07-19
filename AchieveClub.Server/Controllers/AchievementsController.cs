using AchieveClub.Server.Services;
using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController(ApplicationContext db, AchievementStatisticsService achievementStatistics) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly AchievementStatisticsService _achievementStatistics = achievementStatistics;

        [HttpGet]
        public ActionResult<List<AchievementState>> GetAll()
        {
            return _db.Achievements.ToList().Select(a => a.ToState(_achievementStatistics.GetCompletionRatioById(a.Id), CultureInfo.CurrentCulture.Name)).ToList();
        }
    }
}
