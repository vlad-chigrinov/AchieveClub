using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace AchieveClub.Server.Services
{
    public class AchievementStatisticsService(HybridCache cache, ApplicationContext db, ILogger<AchievementStatisticsService> logger, IOutputCacheStore store)
    {
        private readonly HybridCache _cache = cache;
        private readonly ApplicationContext _db = db;
        private readonly ILogger<AchievementStatisticsService> _logger = logger;
        private readonly IOutputCacheStore _store = store;

        public async ValueTask<int> GetCompletionRatioById(int id)
        {
            return await _cache.GetOrCreateAsync($"achievement:{id}", async _ => await CalculateCompletionRatio(id));
        }

        private async ValueTask<int> CalculateCompletionRatio(int id)
        {
            int completedAchievementCount = await _db.CompletedAchievements
                .Include(ca => ca.User)
                .ThenInclude(u => u!.Role)
                .Where(ca => ca.AchieveRefId == id && ca.User!.Role.Title == "Student")
                .CountAsync();
            int studentsCount = await _db.Users
                .Include(u => u.Role)
                .Where(u=> u.Role.Title == "Student")
                .CountAsync();
            if(completedAchievementCount == 0 || studentsCount == 0)
            {
                return 0;
            }
            else
            {
                double ratio = completedAchievementCount / (double)studentsCount;
                return (int)Math.Round((ratio * 100));
            }
        }

        public async Task Clear(int id)
        {
            await _cache.RemoveAsync($"achievement:{id}");
            await _store.EvictByTagAsync("achievements", CancellationToken.None);
        }
    }
}
