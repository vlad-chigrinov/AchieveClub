using Microsoft.AspNetCore.Mvc;

namespace AchieveClub.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubsController(ApplicationContext db) : ControllerBase
    {
        private ApplicationContext _db = db;

        public record ClubName(int Id, string Title);

        [HttpGet("titles")]
        public ActionResult<List<ClubName>> GetClubTitles()
        {
            return _db.Clubs.Select(c => new ClubName(c.Id, c.Title)).ToList();
        }
    }
}