using AchieveClub.Server.Contract.Request;
using AchieveClub.Server.Services;
using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController(
        ApplicationContext db,
        ILogger<AchievementsController> logger,
        AchievementStatisticsService achievementStatistics,
        UserStatisticsService userStatistics,
        CompletedAchievementsCache completedAchievementsCache
        ) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly ILogger<AchievementsController> _logger = logger;
        private readonly AchievementStatisticsService _achievementStatistics = achievementStatistics;
        private readonly UserStatisticsService _userStatistics = userStatistics;
        private readonly CompletedAchievementsCache _completedAchievementsCache = completedAchievementsCache;

        [HttpGet]
        public ActionResult<List<AchievementState>> GetAll()
        {
            return _db.Achievements.ToList().Select(a => a.ToState(_achievementStatistics.GetCompletionRatioById(a.Id), CultureInfo.CurrentCulture.Name)).ToList();
        }

        [HttpGet("{achieve_id:int}")]
        public ActionResult<AchievementDbo> GetById([FromRoute] int achieve_id)
        {
            var achieve = _db.Achievements.FirstOrDefault(a => a.Id == achieve_id);

            if(achieve == null)
            {
                var error = $"Achievement with name: {achieve_id} not exists";
                _logger.LogWarning(error);
                return BadRequest(error);
            }

            return Ok(achieve);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<int> Create([FromBody] CreateAchievementRequest model)
        {
            if (_db.Achievements.Any(a => a.Title_en == model.Title_en))
            {
                var error = $"Achievement with name: {model.Title_en} already exists";
                _logger.LogWarning(error);
                return BadRequest(error);
            }

            var newAchievement = new AchievementDbo
            {
                Title_en = model.Title_en,
                Title_ru = model.Title_ru,
                Title_pl = model.Title_pl,
                Description_en = model.Description_en,
                Description_ru = model.Description_ru,
                Description_pl = model.Description_pl,
                LogoURL = model.LogoURL,
                Xp = model.Xp,
                IsMultiple = model.IsMultiple
            };

            _db.Achievements.Add(newAchievement);

            if (_db.SaveChanges() == 0)
            {
                var error = "Error on add entity to db";
                _logger.LogError(error);
                return BadRequest(error);
            }

            return Ok(newAchievement.Id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{achieveId:int}")]
        public ActionResult Update([FromRoute] int achieveId, [FromBody] CreateAchievementRequest request)
        {
            var achievement = _db.Achievements.FirstOrDefault(a => a.Id == achieveId);

            if (achievement == null)
            {
                var error = $"Achievement with id {achieveId} not found!";
                _logger.LogWarning(error);
                return BadRequest(error);
            }

            var isMultipleChanged = request.IsMultiple != achievement.IsMultiple;
            var isXpChanged = request.Xp != achievement.Xp;

            List<int> userIds = null;
            if (isMultipleChanged || isXpChanged)
            {
                userIds = _db.CompletedAchievements.Where(ca => ca.AchieveRefId == achieveId).Select(ca => ca.UserRefId).Distinct().ToList();
            }

            achievement.Title_en = request.Title_en;
            achievement.Title_ru = request.Title_ru;
            achievement.Title_pl = request.Title_pl;
            achievement.Description_en = request.Description_en;
            achievement.Description_ru = request.Description_ru;
            achievement.Description_en = request.Description_pl;
            achievement.LogoURL = request.LogoURL;
            achievement.Xp = request.Xp;
            achievement.IsMultiple = request.IsMultiple;

            _db.Update<AchievementDbo>(achievement);

            if (_db.SaveChanges() == 0)
            {
                var error = "Error on update entity on db";
                _logger.LogError(error);
                return BadRequest(error);
            }

            if (isMultipleChanged)
            {
                userIds.ForEach(id => _completedAchievementsCache.UpdateByUserId(id));
            }

            if (isXpChanged)
            {
                userIds.ForEach(id => _userStatistics.UpdateXpSumById(id));
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{achieveId:int}")]
        public ActionResult Delete([FromRoute] int achieveId)
        {
            var achievement = _db.Achievements.FirstOrDefault(a => a.Id == achieveId);

            if (achievement == null)
            {
                var error = $"Achievement with id {achieveId} not found!";
                _logger.LogWarning(error);
                return BadRequest(error);
            }

            var userIds = _db.CompletedAchievements.Where(ca => ca.AchieveRefId == achieveId).Select(ca => ca.UserRefId).Distinct().ToList();

            _db.Remove<AchievementDbo>(achievement);

            if (_db.SaveChanges() == 0)
            {
                var error = "Error on delete entity on db";
                _logger.LogError(error);
                return BadRequest(error);
            }

            userIds.ForEach(id => _completedAchievementsCache.UpdateByUserId(id));
            userIds.ForEach(id => _userStatistics.UpdateXpSumById(id));

            return Ok();
        }
    }
}
