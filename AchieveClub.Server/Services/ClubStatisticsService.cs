using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Services
{
    public class ClubStatisticsService(IMemoryCache cache, ApplicationContext db, UserStatisticsService userStatistics)
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ApplicationContext _db = db;
        private readonly UserStatisticsService _userStatistics = userStatistics;

        public int GetAvgXpById(int id)
        {
            if (_cache.TryGetValue<int>($"club:{id}", out var avgXp))
            {
                return avgXp;
            }
            else
            {
                return UpdateAvgXpById(id);
            }
        }

        public int UpdateAvgXpById(int id)
        {
            int calculatedAvgXp = CalculateAvgXp(id);
            _cache.Set<int>($"club:{id}", calculatedAvgXp);
            return calculatedAvgXp;
        }

        private int CalculateAvgXp(int id)
        {
            if (_db.Users.Count(c=>c.ClubRefId == id) == 0)
                return 0;

            double avgXp = _db.Clubs
                .Where(c=>c.Id == id)
                .Include(c => c.Users)
                .ThenInclude(u=>u.Role)
                .First()
                .Users
                .Where(u=>u.Role.Title=="Student")
                .Average(u=>_userStatistics.GetXpSumById(u.Id));
            return (int)Math.Round(avgXp);
        }
    }
}
