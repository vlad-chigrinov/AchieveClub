using AchieveClub.Server.ApiContracts.Categories.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ApplicationContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SmallCategoryResponse>>> GetAll()
    {
        return await db.Categories
            .Where(c => 
                c.StartDate == null || c.EndDate == null ? true
                : c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now)
            .Select(c => new SmallCategoryResponse(c.Id, c.Title, c.Color, c.StartDate, c.EndDate))
            .ToListAsync();
    }
}