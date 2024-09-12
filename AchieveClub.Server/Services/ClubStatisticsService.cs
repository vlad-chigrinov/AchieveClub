using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AchieveClub.Server.Services
{
    public class ClubStatisticsService(IDistributedCache cache, ApplicationContext db, UserStatisticsService userStatistics)
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ApplicationContext _db = db;
        private readonly UserStatisticsService _userStatistics = userStatistics;

        public int GetAvgXpById(int id)
        {
            var avgXp = _cache.GetString($"club:{id}");
            if (avgXp != null)
            {
                return int.Parse(avgXp);
            }
            else
            {
                return UpdateAvgXpById(id);
            }
        }

        public int UpdateAvgXpById(int id)
        {
            int calculatedAvgXp = CalculateAvgXp(id);
            _cache.SetString($"club:{id}", calculatedAvgXp.ToString());
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
