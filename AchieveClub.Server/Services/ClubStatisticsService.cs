using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AchieveClub.Server.Services
{
    public class ClubStatisticsService(
        ILogger<ClubStatisticsService> logger,
        IDistributedCache cache,
        ApplicationContext db,
        UserStatisticsService userStatistics)
    {
        private readonly ILogger<ClubStatisticsService> _logger = logger;
        private readonly IDistributedCache _cache = cache;
        private readonly ApplicationContext _db = db;
        private readonly UserStatisticsService _userStatistics = userStatistics;

        public int GetAvgXpById(int id)
        {
            var avgXp = _cache.GetString($"club:{id}");
            if (avgXp != null)
            {
                _logger.LogDebug($"Get value from cache: club:{id}");
                return int.Parse(avgXp);
            }
            else
            {
                _logger.LogDebug($"Update value in cache: club:{id}");
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
            if (_db.Users
                .Include(u => u.Role)
                .Count(u=>u.ClubRefId == id && u.Role.Title == "Student") == 0)
                return 0;

            double avgXp = _db.Users
                .Include(u => u.Role)
                .Where(u=>u.ClubRefId == id)
                .Where(u=>u.Role.Title=="Student")
                .ToList()
                .Average(u=>_userStatistics.GetXpSumById(u.Id));
            return (int)Math.Round(avgXp);
        }
    }
}
