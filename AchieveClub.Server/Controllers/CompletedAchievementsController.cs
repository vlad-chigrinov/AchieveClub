using AchieveClub.Server.RepositoryItems;
using AchieveClubServer.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompletedAchievementsController(ApplicationContext db) : ControllerBase
    {
        private ApplicationContext _db = db;

        [Authorize]
        [HttpGet]
        public ActionResult<List<CompletedAchievementState>> GetByUserId()
        {
            var cookie = Request.Cookies["X-User-Id"];
            if (cookie == null || int.TryParse(cookie, out int userId) == false)
                return BadRequest("User not found!");

            if (_db.Users.Count(x => x.Id == userId) == 0)
                return BadRequest("User not found!");

            return _db.CompletedAchievements.Where(ca => ca.UserRefId == userId).Select(ca => new CompletedAchievementState(ca.AchieveRefId)).ToList();
        }

        [Authorize(Roles = "Supervisor, Admin")]
        [HttpPost]
        public ActionResult CompleteAchievement(CompleteAchievementModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == model.UserId);
            var achievement = _db.Achievements.FirstOrDefault(u => u.Id == model.AchievementId);

            if (user == null || achievement == null)
                return BadRequest("UserId/AchieveId is invalid");

            if (_db.CompletedAchievements.Count(ca => ca.UserRefId == user.Id && ca.AchieveRefId == achievement.Id) != 0)
                return BadRequest("Multiple achievements not supported");

            var supervisorId = int.Parse(HttpContext.User.Identities.First().Name);

            _db.CompletedAchievements.Add(new CompletedAchievementDbo { UserRefId = user.Id, AchieveRefId = achievement.Id, DateOfCompletion = DateTime.Now, SupervisorRefId = supervisorId });

            _db.SaveChanges();

            return Ok();
        }

        public record CompleteAchievementModel([Required, Range(1, double.PositiveInfinity)] int UserId, [Required, Range(1, double.PositiveInfinity)] int AchievementId);
    }
}
