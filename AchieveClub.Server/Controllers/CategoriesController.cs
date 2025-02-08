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
            .Select(c => new SmallCategoryResponse(c.Id, c.Title, c.Color, c.EndDate))
            .ToListAsync();
    }
}