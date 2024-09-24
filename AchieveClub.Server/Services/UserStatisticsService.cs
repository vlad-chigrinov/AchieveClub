using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AchieveClub.Server.Services
{
    public class UserStatisticsService(IDistributedCache cache, ApplicationContext db)
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ApplicationContext _db = db;

        public int GetXpSumById(int id)
        {
            var xpSum = _cache.GetString($"user:{id}");
            if (xpSum != null)
            {
                return int.Parse(xpSum);
            }
            else
            {
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
