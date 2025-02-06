using AchieveClub.Server.RepositoryItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(
        ApplicationContext db,
        ILogger<UsersController> logger
        ) : ControllerBase
    {
        public record ChangeRoleRequest([Required] int userId, [Required] int roleId);

        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<UserState>> GetCurrent()
        {
            var userIdString = HttpContext.User.Identity?.Name;
            if (userIdString == null || int.TryParse(userIdString, out int userId) == false)
            {
                logger.LogWarning("Access token not contains userId or userId is the wrong format: {userIdString}", userIdString);
                return NotFound($"Access token not contains userId or userId is the wrong format: {userIdString}");
            }
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                logger.LogWarning("User with userId:{userId} not found", userId);
                return NotFound($"User with userId:{userId} not found");
            }

            var xpSum = await db.CompletedAchievements
                .Where(ca => ca.UserRefId == userId)
                .Include(ca => ca.Achievement)
                .SumAsync(ca => ca.Achievement.Xp);

            return user.ToUserState(xpSum);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserState>> GetById([FromRoute] int userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                logger.LogWarning("User with userId:{userId} not found", userId);
                return NotFound($"User with userId:{userId} not found");
            }

            var xpSum = await db.CompletedAchievements
                .Where(ca => ca.UserRefId == userId)
                .Include(ca => ca.Achievement)
                .SumAsync(ca => ca.Achievement.Xp);

            return user.ToUserState(xpSum);
        }

        [HttpGet]
        [OutputCache(Duration = (3 * 60), Tags = ["users"])]
        public async Task<ActionResult<List<UserState>>> GetStudents()
        {
            return await db.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Title == "Student")
                .Select(u => u.ToUserState(db.CompletedAchievements
                    .Where(ca => ca.UserRefId == u.Id)
                    .Include(ca => ca.Achievement)
                    .Sum(ca => ca.Achievement.Xp)))
                .ToListAsync();
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<UserState>>> GetAll(CancellationToken ct)
        {
            return await db.Users
                .Select(u => u.ToUserState(db.CompletedAchievements
                    .Where(ca => ca.UserRefId == u.Id)
                    .Include(ca => ca.Achievement)
                    .Sum(ca => ca.Achievement.Xp)))
                .ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                logger.LogWarning("User with userId:{userId} not found", userId);
                return NotFound($"User with userId:{userId} not found");
            }

            db.Users.Remove(user);

            await db.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("change_role")]
        public async Task<ActionResult> ChangeUserRole([FromBody] ChangeRoleRequest request)
        {
            if (db.Roles.Any(r => r.Id == request.roleId) == false)
            {
                logger.LogWarning("Role with roleId:{request.roleId} not found", request.roleId);
                return NotFound($"Role with roleId:{request.roleId} not found");
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.userId);
            if (user == null)
            {
                logger.LogWarning("User with userId:{request.userId} not found", request.userId);
                return NotFound($"User with userId:{request.userId} not found");
            }

            if (user.RoleRefId == request.roleId)
            {
                logger.LogWarning("This user:{request.userId} already has this role:{request.roleId}", request.userId, request.roleId);
                return BadRequest($"This user:{request.userId} already has this role:{request.roleId}");
            } 

            user.RoleRefId = request.roleId;

            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
