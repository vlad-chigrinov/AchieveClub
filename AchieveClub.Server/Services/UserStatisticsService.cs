using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AchieveClub.Server.Services
{
    public class UserStatisticsService(ILogger<UserStatisticsService> logger, IDistributedCache cache, ApplicationContext db)
    {
        private readonly ILogger<UserStatisticsService> _logger = logger;
        private readonly IDistributedCache _cache = cache;
        private readonly ApplicationContext _db = db;

        public int GetXpSumById(int id)
        {
            var xpSum = _cache.GetString($"user:{id}");
            if (xpSum != null)
            {
                _logger.LogDebug($"Get user:{id} from cache");
                return int.Parse(xpSum);
            }
            else
            {
                _logger.LogDebug($"Update user:{id} in cache");
                return UpdateXpSumById(id);
            }
        }

        public int UpdateXpSumById(int id)
        {
            int calculatedXpSum = CalculateXpSum(id);
            _cache.SetString($"user:{id}", calculatedXpSum.ToString());
            return calculatedXpSum;
        }

        private int CalculateXpSum(int id)
        {
            return _db.CompletedAchievements
                .Where(ca => ca.UserRefId == id)
                .Include(ca=>ca.Achievement)
                .Select(ca=>ca.Achievement.Xp)
                .Sum();
        }

        public void DeleteXpSumById(int id)
        {
            _cache.Remove($"user:{id}");
        }
    }
}
