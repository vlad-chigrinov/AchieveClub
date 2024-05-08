using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Services
{
    public class UserStatisticsSevice(IMemoryCache cache, ApplicationContext db)
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ApplicationContext _db = db;

        public int GetXpSumById(int id)
        {
            if (_cache.TryGetValue<int>($"user:{id}", out var xpSum))
            {
                return xpSum;
            }
            else
            {
                int calculatedXpSum = CalculateXpSum(id);
                _cache.Set<int>($"user:{id}", calculatedXpSum);
                return calculatedXpSum;
            }
        }

        private int CalculateXpSum(int id)
        {
            return _db.CompletedAchievements
                .Where(ca => ca.UserRefId == id)
                .Include(ca=>ca.Achievement)
                .Select(ca=>ca.Achievement.Xp)
                .Sum();
        }
    }
}
