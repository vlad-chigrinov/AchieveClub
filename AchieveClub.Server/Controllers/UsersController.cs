using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(
        ApplicationContext db,
        UserStatisticsService userStatistics,
        AchievementStatisticsService achievementStatistics,
        ClubStatisticsService clubStatistics
        ) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly UserStatisticsService _userStatistics = userStatistics;
        private readonly AchievementStatisticsService _achievementsStatistics = achievementStatistics;
        private readonly ClubStatisticsService _clubStatistics = clubStatistics;

        public record ChangeRoleRequest([Required] int UserId, [Required] int RoleId);

        [Authorize]
        [HttpGet("current")]
        public ActionResult<UserState> GetCurrent()
        {
            var userName = HttpContext.User.Identity?.Name;
            if (userName == null || int.TryParse(userName, out int userId) == false)
                return Unauthorized("User not found!");

            var result = _db.Users.Include(u => u.Club).FirstOrDefault(u => u.Id == userId);
            if (result == null)
            {
                return Unauthorized("User not found!");
            }
            else
            {
                return result.ToUserState(_userStatistics.GetXpSumById(result.Id), CultureInfo.CurrentCulture.Name);
            }
        }

        [HttpGet("{userId}")]
        [OutputCache(Duration = (10 * 60), Tags = ["users"], VaryByRouteValueNames = ["userId"])]
        public ActionResult<UserState> GetById([FromRoute] int userId)
        {
            var result = _db.Users.Include(u => u.Club).FirstOrDefault(u => u.Id == userId);
            if (result == null)
            {
                return BadRequest("User not found!");
            }
            else
            {
                return result.ToUserState(_userStatistics.GetXpSumById(result.Id), CultureInfo.CurrentCulture.Name);
            }
        }

        [HttpGet]
        [OutputCache(Duration = (10 * 60), Tags = ["users"])]
        public ActionResult<List<UserState>> GetStudents()
        {
            return _db.Users
                .Include(u => u.Club)
                .Include(u => u.Role)
                .Where(u=>u.Role.Title == "Student")
                .ToList()
                .Select(u => u.ToUserState(_userStatistics.GetXpSumById(u.Id), CultureInfo.CurrentCulture.Name))
                .ToList();
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<UserState>>> GetAll(CancellationToken ct)
        {
            return (await _db.Users
                .Include(u => u.Club)
                .ToListAsync(ct))  
                .Select(u => u.ToUserState(_userStatistics.GetXpSumById(u.Id), CultureInfo.CurrentCulture.Name))
                .ToList();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return BadRequest("UserId is invalid");

            var userClubId = user.ClubRefId;
            var userCompletedAchievementsIds = _db.CompletedAchievements.Where(ca => ca.UserRefId == userId).Select(sa => sa.Id).ToList();

            _db.Users.Remove(user);
            if (await _db.SaveChangesAsync() > 0)
            {
                foreach (var completedAchievementId in userCompletedAchievementsIds)
                {
                    await _achievementsStatistics.Clear(completedAchievementId);
                }
                _userStatistics.DeleteXpSumById(userId);
                _clubStatistics.UpdateAvgXpById(userClubId);
                return Ok();
            }
            else
            {
                return BadRequest("Error on delete entity from db");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("change_role")]
        public ActionResult ChangeUserRole([FromBody] ChangeRoleRequest model)
        {
            if(_db.Roles.Any(r=>r.Id == model.RoleId) == false)
                return BadRequest("Role with this RoleId does not exist");

            var user = _db.Users.FirstOrDefault(u => u.Id == model.UserId);

            if (user == null)
                return BadRequest("UserId is invalid");

            if (user.RoleRefId == model.RoleId)
                return BadRequest("This role is already in use");

            user.RoleRefId = model.RoleId;

            _db.Update(user);
            if (_db.SaveChanges() > 0)
                return Ok();
            else
                return BadRequest("Error on update entity on db");
        }
    }
}
