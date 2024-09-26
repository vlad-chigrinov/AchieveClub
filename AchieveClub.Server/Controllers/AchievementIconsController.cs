using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AchieveClub.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementIconsController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<string>> GetAll()
        {
            var filePaths = Directory.GetFiles("./wwwroot/icons/achievements/");
            return filePaths.Select(a => a = a.Replace("./wwwroot/", "")).ToList();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var fileInfo = new FileInfo(file.FileName);
            var fileTypes = new List<string> { ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif" };
            if (fileTypes.Contains(fileInfo.Extension) == false || fileInfo.Extension.IsNullOrEmpty())
                return BadRequest("File not an image");

            var filePath = $"/icons/achievements/{Path.GetFileNameWithoutExtension(fileInfo.Name)}.jpeg";

            if (Path.Exists($"./wwwroot{filePath}"))
                return BadRequest("File with this name already exists");

            using (var readStream = file.OpenReadStream())
            {
                var image = await Image.LoadAsync(readStream);

                image.Mutate(x => x.Resize(new ResizeOptions()
                {
                    Size = new Size(600, 600),
                    Mode = ResizeMode.Crop
                }));

                using (var fileStream = new FileStream($"./wwwroot{filePath}", FileMode.CreateNew, FileAccess.Write))
                {
                    await image.SaveAsJpegAsync(fileStream);
                }
            }
            return Ok(filePath);
        }
    }
}
