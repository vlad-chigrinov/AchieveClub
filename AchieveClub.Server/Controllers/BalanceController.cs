using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BalanceController(ILogger<BalanceController> logger, ApplicationContext db) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<int>> GetCurrentUserBalance()
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

        return user.Balance;
    }
}