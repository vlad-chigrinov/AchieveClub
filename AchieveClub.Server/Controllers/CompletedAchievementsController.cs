using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompletedAchievementsController(ApplicationContext db, AchievementStatisticsService achievementStatistics, UserStatisticsService userStatistics, ClubStatisticsService clubStatistics) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly AchievementStatisticsService _achievementStatistics = achievementStatistics;
        private readonly UserStatisticsService _userStatistics = userStatistics;
        private readonly ClubStatisticsService _clubStatistics = clubStatistics;

        [Authorize]
        [HttpGet("current")]
        public ActionResult<List<CompletedAchievementState>> GetForCurrentUser()
        {
            var cookie = Request.Cookies["X-User-Id"];
            if (cookie == null || int.TryParse(cookie, out int userId) == false)
                return BadRequest("User not found!");

            if (_db.Users.Count(x => x.Id == userId) == 0)
                return BadRequest("User not found!");

            return _db.CompletedAchievements.Where(ca => ca.UserRefId == userId).Select(ca => new CompletedAchievementState(ca.AchieveRefId)).ToList();
        }

        [HttpGet("{userId}")]
        public ActionResult<List<CompletedAchievementState>> GetForCurrentUser([FromRoute] int userId)
        {
            if (_db.Users.Count(x => x.Id == userId) == 0)
                return BadRequest("User not found!");

            return _db.CompletedAchievements.Where(ca => ca.UserRefId == userId).Select(ca => new CompletedAchievementState(ca.AchieveRefId)).ToList();
        }

        [Authorize(Roles = "Supervisor, Admin")]
        [HttpDelete]
        public ActionResult CalcelCompleteAchievements(CompleteAchievementModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == model.UserId);

            if (user == null)
                return BadRequest("UserId is invalid");

            foreach(var achieveId in model.AchievementIds)
            {
                var achievement = _db.CompletedAchievements.FirstOrDefault(ca=>ca.UserRefId == model.UserId && ca.AchieveRefId == achieveId);

                if(achievement == null)
                    return BadRequest("One of AchieveIds is invalid or not completed!");

                _db.CompletedAchievements.Remove(achievement);
            }   

            var supervisorId = int.Parse(HttpContext.User.Identities.First().Name);

            _db.SaveChanges();

            _userStatistics.UpdateXpSumById(model.UserId);
            _clubStatistics.UpdateAvgXpById(user.ClubRefId);
            foreach( var achievementId in model.AchievementIds)
                _achievementStatistics.UpdateCompletedRatioById(achievementId);

            return Ok();
        }

        [Authorize(Roles = "Supervisor, Admin")]
        [HttpPost]
        public ActionResult CompleteAchievements(CompleteAchievementModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == model.UserId);

            if (user == null)
                return BadRequest("UserId is invalid");

            var achievements = new List<AchievementDbo>();
            foreach(var achieveId in model.AchievementIds)
            {
                var achievement = _db.Achievements.FirstOrDefault(u => u.Id == achieveId);

                if(achievement == null)
                    return BadRequest("One of AchieveIds is invalid!");

                if (_db.CompletedAchievements.Count(ca => ca.UserRefId == user.Id && ca.AchieveRefId == achievement.Id) != 0)
                    return BadRequest("Multiple achievements not supported");

                achievements.Add(achievement);
            }   

            var supervisorId = int.Parse(HttpContext.User.Identities.First().Name);

            foreach(var achievement in achievements)
                _db.CompletedAchievements.Add(new CompletedAchievementDbo { UserRefId = user.Id, AchieveRefId = achievement.Id, DateOfCompletion = DateTime.Now, SupervisorRefId = supervisorId });

            _db.SaveChanges();

            _userStatistics.UpdateXpSumById(model.UserId);
            _clubStatistics.UpdateAvgXpById(user.ClubRefId);
            foreach( var achievementId in model.AchievementIds)
                _achievementStatistics.UpdateCompletedRatioById(achievementId);

            return Ok();
        }

        public record CompleteAchievementModel([Required, Range(1, double.PositiveInfinity)] int UserId, [Required] List<int> AchievementIds);
    }
}
