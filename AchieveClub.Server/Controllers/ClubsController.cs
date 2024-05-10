using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AchieveClub.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubsController(ApplicationContext db, ClubStatisticsService clubStatistics) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly ClubStatisticsService _clubStatistics = clubStatistics;

        public record ClubName(int Id, string Title);

        [HttpGet]
        public ActionResult<List<ClubState>> GetAll()
        {
            return _db.Clubs.ToList().Select(c => c.ToState(_clubStatistics.GetAvgXpById(c.Id))).ToList();
        }

        [HttpGet("titles")]
        public ActionResult<List<ClubName>> GetClubTitles()
        {
            return _db.Clubs.Select(c => new ClubName(c.Id, c.Title)).ToList();
        }
    }
}