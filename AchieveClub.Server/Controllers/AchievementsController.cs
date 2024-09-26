using AchieveClub.Server.Services;
using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController(ApplicationContext db, AchievementStatisticsService achievementStatistics) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly AchievementStatisticsService _achievementStatistics = achievementStatistics;

        public record CreateAchievementRequest(
            [Required, StringLength(100, MinimumLength = 5)] string Title_en,
            [Required, StringLength(100, MinimumLength = 5)] string Title_ru,
            [Required, StringLength(100, MinimumLength = 5)] string Title_pl,
            [Required, StringLength(300, MinimumLength = 5)] string Description_en,
            [Required, StringLength(300, MinimumLength = 5)] string Description_ru,
            [Required, StringLength(300, MinimumLength = 5)] string Description_pl,
            [Required] string LogoURL,
            [Required, Range(0, double.MaxValue)] int Xp
        );

        [HttpGet]
        public ActionResult<List<AchievementState>> GetAll()
        {
            return _db.Achievements.ToList().Select(a => a.ToState(_achievementStatistics.GetCompletionRatioById(a.Id), CultureInfo.CurrentCulture.Name)).ToList();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<int> Create([FromBody] CreateAchievementRequest model)
        {
            if (_db.Achievements.Any(a => a.Title_en == model.Title_en))
                return BadRequest("Achievement with this name already exists");

            var newAchievement = new AchievementDbo
            {
                Title_en = model.Title_en,
                Title_ru = model.Title_ru,
                Title_pl = model.Title_pl,
                Description_en = model.Description_en,
                Description_ru = model.Description_ru,
                Description_pl = model.Description_pl,
                LogoURL = model.LogoURL,
                Xp = model.Xp
            };

            _db.Achievements.Add(newAchievement);

            if (_db.SaveChanges() == 0)
                return BadRequest("Error on add entity to db");

            return Ok(newAchievement.Id);
        }
    }
}
