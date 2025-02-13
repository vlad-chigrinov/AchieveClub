using AchieveClub.Server.RepositoryItems;
using Microsoft.AspNetCore.Mvc;

namespace AchieveClub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(ApplicationContext db) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<TagDbo>> Get()
    {
        return db.Tags.ToList();
    }
}