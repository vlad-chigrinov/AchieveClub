using AchieveClub.Server.Contract.Request;
using AchieveClub.Server.Services;
using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        [OutputCache(Duration = (10 * 60),Tags = ["achievements"], VaryByRouteValueNames = ["userId"])]
        public async Task<ActionResult<List<AchievementState>>> GetAll()
        {
            var users = await db.Achievements.ToListAsync();
            var userStates = users.Select(a => a.ToState(achievementStatistics.GetCompletionRatioById(a.Id).Result, CultureInfo.CurrentCulture.Name));
            return Ok(userStates.OrderBy(a=>a.Xp).ToList());
        }

        [HttpGet("{achieveId:int}")]
        public ActionResult<AchievementDbo> GetById([FromRoute] int achieveId)
        {
            var achieve = db.Achievements.FirstOrDefault(a => a.Id == achieveId);

            if(achieve == null)
            {
                var error = $"Achievement with name: {achieveId} not exists";
                logger.LogWarning(error);
                return BadRequest(error);
            }

            return Ok(achieve);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<int> Create([FromBody] CreateAchievementRequest model)
        {
            if (db.Achievements.Any(a => a.Title_en == model.Title_en))
            {
                var error = $"Achievement with name: {model.Title_en} already exists";
                logger.LogWarning(error);
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

            db.Achievements.Add(newAchievement);

            if (db.SaveChanges() == 0)
            {
                var error = "Error on add entity to db";
                logger.LogError(error);
                return BadRequest(error);
            }

            return Ok(newAchievement.Id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{achieveId:int}")]
        public ActionResult Update([FromRoute] int achieveId, [FromBody] CreateAchievementRequest request)
        {
            var achievement = db.Achievements.FirstOrDefault(a => a.Id == achieveId);

            if (achievement == null)
            {
                var error = $"Achievement with id {achieveId} not found!";
                logger.LogWarning(error);
                return BadRequest(error);
            }

            var isMultipleChanged = request.IsMultiple != achievement.IsMultiple;
            var isXpChanged = request.Xp != achievement.Xp;

            List<int> userIds = Enumerable.Empty<int>().ToList();
            if (isMultipleChanged || isXpChanged)
            {
                userIds = db.CompletedAchievements.Where(ca => ca.AchieveRefId == achieveId).Select(ca => ca.UserRefId).Distinct().ToList();
            }

            achievement.Title_en = request.Title_en;
            achievement.Title_ru = request.Title_ru;
            achievement.Title_pl = request.Title_pl;
            achievement.Description_en = request.Description_en;
            achievement.Description_ru = request.Description_ru;
            achievement.Description_en = request.Description_en;
            achievement.LogoURL = request.LogoURL;
            achievement.Xp = request.Xp;
            achievement.IsMultiple = request.IsMultiple;

            db.Update(achievement);

            if (db.SaveChanges() == 0)
            {
                var error = "Error on update entity on db";
                logger.LogError(error);
                return BadRequest(error);
            }

            if (isMultipleChanged)
            {
                userIds.ForEach(id => completedAchievementsCache.UpdateByUserId(id));
            }

            if (isXpChanged)
            {
                userIds.ForEach(id => userStatistics.UpdateXpSumById(id));
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{achieveId:int}")]
        public ActionResult Delete([FromRoute] int achieveId)
        {
            var achievement = db.Achievements.FirstOrDefault(a => a.Id == achieveId);

            if (achievement == null)
            {
                var error = $"Achievement with id {achieveId} not found!";
                logger.LogWarning(error);
                return BadRequest(error);
            }

            var userIds = db.CompletedAchievements.Where(ca => ca.AchieveRefId == achieveId).Select(ca => ca.UserRefId).Distinct().ToList();

            db.Remove(achievement);

            if (db.SaveChanges() == 0)
            {
                var error = "Error on delete entity on db";
                logger.LogError(error);
                return BadRequest(error);
            }

            userIds.ForEach(id => completedAchievementsCache.UpdateByUserId(id));
            userIds.ForEach(id => userStatistics.UpdateXpSumById(id));

            return Ok();
        }
    }
}
