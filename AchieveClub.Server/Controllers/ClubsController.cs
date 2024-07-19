using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AchieveClub.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubsController(
        ApplicationContext db,
        ClubStatisticsService clubStatistics,
        UserStatisticsService userStatistics
        ) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly ClubStatisticsService _clubStatistics = clubStatistics;
        private readonly UserStatisticsService _userStatistics = userStatistics;

        [HttpGet]
        public ActionResult<List<SmallClubState>> GetAll()
        {
            return _db.Clubs.ToList().Select(c => c.ToSmallState(_clubStatistics.GetAvgXpById(c.Id), CultureInfo.CurrentCulture.Name)).ToList();
        }

        [HttpGet("{clubId}")]
        public ActionResult<ClubState> GetById([FromRoute] int clubId)
        {
            var club = _db.Clubs.Include(c=>c.Users).FirstOrDefault(c => c.Id == clubId);

            if (club == null)
                return BadRequest("Club not found!");

            var users = club.Users.Select(u => u.ToUserState(_userStatistics.GetXpSumById(u.Id), CultureInfo.CurrentCulture.Name)).ToList();
            var clubState = club.ToState(_clubStatistics.GetAvgXpById(club.Id), users, CultureInfo.CurrentCulture.Name);

            return clubState;
        }

        [HttpGet("titles")]
        public ActionResult<List<ClubName>> GetClubTitles()
        {
            return _db.Clubs.Select(c => c.ToTitleState(CultureInfo.CurrentCulture.Name)).ToList();
        }
    }
}