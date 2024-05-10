using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Services
{
    public class AchievementStatisticsService(IMemoryCache cache, ApplicationContext db)
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ApplicationContext _db = db;

        public int GetCompletionRatioById(int id)
        {
            if(_cache.TryGetValue<int>($"achievement:{id}", out var ratio))
            {
                return ratio;
            }
            else
            {
                return UpdateCompletedRatioById(id);
            }
        }

        public int UpdateCompletedRatioById(int id)
        {
            int calculatedRatio = CalculateCompletionRatio(id);
            _cache.Set<int>($"achievement:{id}", calculatedRatio);
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
