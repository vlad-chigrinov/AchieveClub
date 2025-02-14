using AchieveClub.Server.RepositoryItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AchieveClub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(ApplicationContext db) : ControllerBase
{
    [HttpGet]
    [OutputCache(Duration = (3 * 60), Tags = ["achievements"])]
    public ActionResult<List<TagDbo>> Get()
    {
        return db.Tags.ToList();
    }
}