using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AchieveClub.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController(ApplicationContext db, ILogger<AvatarController> logger) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
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

            if (file.Length == 0)
            {
                logger.LogWarning("No file uploaded");
                return BadRequest("No file uploaded");
            }

            if (file.Length > 10_000_000)
            {
                logger.LogWarning("File it too long: {file.Length} bytes", file.Length);
                return BadRequest($"File it too long: {file.Length} bytes");
            }

            var fileInfo = new FileInfo(file.FileName);

            if (string.IsNullOrWhiteSpace(fileInfo.Extension))
            {
                logger.LogWarning("File extension not found: {file.FileName}", file.FileName);
                return BadRequest($"File extension not found: {file.FileName}");
            }

            var fileTypes = new List<string> { ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif" };

            if (fileTypes.Contains(fileInfo.Extension.ToLower()) == false)
            {
                logger.LogWarning("File extension not supported: {fileInfo.Extension}. Supported extensions: {fileTypes}", fileInfo.Extension, fileTypes);
                return BadRequest($"File extension not supported: {fileInfo.Extension}. Supported extensions: {fileTypes.Aggregate((a, b) => $"{a},{b}")}");
            }

            var filePath = $"avatars/{Guid.NewGuid()}.jpeg";

            if (Path.Exists($"./wwwroot/{filePath}"))
            {
                logger.LogWarning("File with this name already exists: {filePath}", filePath);
                return BadRequest($"File with this name already exists: {filePath}");
            }

            using (var readStream = file.OpenReadStream())
            {
                var image = await Image.LoadAsync(readStream);

                image.Mutate(x => x.Resize(new ResizeOptions()
                {
                    Size = new Size(600, 600),
                    Mode = ResizeMode.Crop
                }));

                using (var fileStream = new FileStream($"./wwwroot/{filePath}", FileMode.CreateNew, FileAccess.Write))
                {
                    await image.SaveAsJpegAsync(fileStream);
                }
            }

            logger.LogInformation("File saved as .jpeg on: {filePath}", filePath);

            user.Avatar = filePath;
            await db.SaveChangesAsync();

            logger.LogInformation("User avatar changed. User: {user}", user);

            return Ok(filePath);
        }
    }
}
