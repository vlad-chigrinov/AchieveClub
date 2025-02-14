using AchieveClub.Server.ApiContracts.Categories.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ApplicationContext db) : ControllerBase
{
    [HttpGet]
    [OutputCache(Duration = (3 * 60), Tags = ["achievements"])]
    public async Task<ActionResult<List<SmallCategoryResponse>>> GetAll()
    {
        var categories = await db.Categories.ToListAsync();

        return categories
            .Select(category =>
            {
                var available = category.StartDate == null || category.EndDate == null ||
                                category.StartDate <= DateTime.Now && category.EndDate >= DateTime.Now;
                return new SmallCategoryResponse(
                    category.Id,
                    category.Title,
                    category.Color,
                    category.StartDate,
                    category.EndDate,
                    available ? category.AvailableBanner : category.UnavailableBanner,
                    available
                );
            })
            .ToList();
    }
}