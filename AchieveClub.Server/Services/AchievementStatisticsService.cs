using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AchieveClub.Server.Services
{
    public class AchievementStatisticsService(IDistributedCache cache, ApplicationContext db, ILogger<AchievementStatisticsService> logger)
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ApplicationContext _db = db;
        private readonly ILogger<AchievementStatisticsService> _logger = logger;

        public int GetCompletionRatioById(int id)
        {
            var ratio = _cache.GetString($"achievement:{id}");
            if (ratio != null)
            {
                _logger.LogDebug($"Get value from cache: achievement:{id}");
                return int.Parse(ratio);
            }
            else
            {
                _logger.LogDebug($"Update value in cache: achievement:{id}");
                return UpdateCompletedRatioById(id);
            }
        }

        public int UpdateCompletedRatioById(int id)
        {
            int calculatedRatio = CalculateCompletionRatio(id);
            _cache.SetString($"achievement:{id}", calculatedRatio.ToString());
            return calculatedRatio;
        }

        private int CalculateCompletionRatio(int id)
        {
            int completedAchievementCount = _db.CompletedAchievements
                .Include(ca => ca.User)
                .ThenInclude(u => u.Role)
                .Where(ca => ca.AchieveRefId == id && ca.User.Role.Title == "Student")
                .Count();
            int studentsCount = _db.Users
                .Include(u => u.Role)
                .Where(u=> u.Role.Title == "Student")
                .Count();
            if(completedAchievementCount == 0 || studentsCount == 0)
            {
                return 0;
            }
            else
            {
                double ratio = (double)completedAchievementCount / (double)studentsCount;
                return (int)Math.Round((ratio * 100));
            }
        }
    }
}
