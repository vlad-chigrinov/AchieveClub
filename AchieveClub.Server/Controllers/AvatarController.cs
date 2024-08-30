using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AchieveClub.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController(ApplicationContext db) : ControllerBase
    {
        private ApplicationContext _db = db;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var cookie = Request.Cookies["X-User-Id"];
            if (cookie == null || int.TryParse(cookie, out int userId) == false)
                return BadRequest("User not found!");

            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return BadRequest("User not found!");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var fileInfo = new FileInfo(file.FileName);
            var fileTypes = new List<string> { ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif" };
            if (fileTypes.Contains(fileInfo.Extension) == false || fileInfo.Extension.IsNullOrEmpty())
                return BadRequest("File not an image.");

            var filePath = $"avatars/{Guid.NewGuid()}.jpeg";

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

            user.Avatar = filePath;
            _db.Users.Update(user);
            _db.SaveChanges();

            return Ok(filePath);
        }
    }
}
